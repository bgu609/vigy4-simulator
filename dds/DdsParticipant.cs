using Omg.Dds.Core;
using Pa5455CmsDds.Component;
using Rti.Dds.Core;
using Rti.Dds.Core.Status;
using Rti.Dds.Domain;
using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Dds.Topics;

namespace Pa5455CmsDds
{
    public class DdsParticipant
    {
        private DomainParticipant substance;
        private Publisher? publisher;
        private Subscriber? subscriber;

        private WaitSet topicWaiter = new WaitSet();
        private Thread? topicWaiterThread;

        private Dictionary<string, Entity> topicEntityCollection = new Dictionary<string, Entity>();
        private Dictionary<string, AnyDataWriter> topicWriterCollection = new Dictionary<string, AnyDataWriter>();
        private Dictionary<string, AnyDataReaderResource> topicReaderResourceCollection = new Dictionary<string, AnyDataReaderResource>();

        public int domain { get; private set; } = -1;

        public bool waitingTopic { get; private set; } = false;
        public Duration topicWaiterDuration { get; private set; } = Duration.FromMilliseconds(10u);



        public DdsParticipant(int domain)
        {
            this.substance = DomainParticipantFactory.Instance.CreateParticipant(domain);
            this.domain = domain;
        }

        public DdsParticipant(int domain, string qosProfileName)
        {
            DomainParticipantQos internalQos = QosProvider.Default.GetDomainParticipantQos("ParticipantQosInternal");

            this.substance = DomainParticipantFactory.Instance.CreateParticipant(domain, internalQos);
            this.domain = domain;
        }



        /// <summary>
        /// Topic 클래스에 대한 Topic명을 반환합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <returns></returns>
        public string getTopicName<T>() where T : IEquatable<T> // (개발 SW기본설계_협력개발용_250819.pdf, 5.DDS 사용 정책, 체계 내부 연동, 명명 규칙) (CSCI명과 CSC명은 제외하고 써야한다는 건가?)
        {
            return typeof(T).Name; // topic class 이름이 실제로 사용하는 topic name이 아니면 이렇게 하면 안 됨 (현재는 topic class name == topic name 인 것으로 인식하고 있는데, 확실하게 하기 위해서는 재차 확인해도 될 듯)
        }

        /// <summary>
        /// Topic 클래스에 대한 Topic Entity 객체를 반환합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <returns></returns>
        public Topic<T> getTopicEntity<T>() where T : IEquatable<T>
        {
            lock (this.topicEntityCollection)
            {
                string topicName = getTopicName<T>();

                if (this.topicEntityCollection.ContainsKey(topicName))
                {
                    return (Topic<T>)this.topicEntityCollection[topicName];
                }
                else
                {
                    Topic<T> topic = this.substance.CreateTopic<T>(topicName);
                    this.topicEntityCollection.Add(topicName, topic);

                    return topic;
                }
            }
        }

        /// <summary>
        /// DDS 참여자가 생성한 모든 Topic Entity 객체를 정리합니다.
        /// </summary>
        public void clearTopicEntityCollection()
        {
            lock (this.topicEntityCollection)
            {
                foreach (Entity topicEntity in this.topicEntityCollection.Values)
                {
                    topicEntity.Dispose();
                }

                this.topicEntityCollection.Clear();
            }
        }


        /// <summary>
        /// DDS 참여자가 특정 Topic명으로 배포권을 보유하고 있는지 확인합니다.
        /// </summary>
        /// <param name="topicName">Topic명</param>
        /// <returns>DDS 참여자가 Topic의 배포권을 보유하고 있는 경우 true, 그렇지 않으면 false를 반환합니다.</returns>
        public bool isPublicationTopic(string topicName)
        {
            lock (this.topicWriterCollection)
            {
                return this.topicWriterCollection.ContainsKey(topicName);
            }
        }

        /// <summary>
        /// DDS 참여자가 특정 Topic 클래스로 배포권을 보유하고 있는지 확인합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <returns>DDS 참여자가 Topic의 배포권을 보유하고 있는 경우 true, 그렇지 않으면 false를 반환합니다.</returns>
        public bool isPublicationTopic<T>() where T : IEquatable<T>
        {
            lock (this.topicWriterCollection)
            {
                string topicName = getTopicName<T>();

                return this.topicWriterCollection.ContainsKey(topicName);
            }
        }

        /// <summary>
        /// [사용 주의] 반환된 DataWriter는 DDS 참여자에 의해 관리되고 있으므로, 외부에서 직접 사용하는 경우 Dispose와 관련된 문제를 유발할 수 있습니다.
        /// <br/>
        /// Topic 클래스에 대한 DataWriter 객체를 반환합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <returns>DDS 참여자가 DataWriter를 보유하고 있지 않으면 null을 반환합니다.</returns>
        public DataWriter<T>? getTopicWriter<T>() where T : IEquatable<T>
        {
            lock (this.topicWriterCollection)
            {
                string topicName = getTopicName<T>();

                if (this.topicWriterCollection.ContainsKey(topicName))
                {
                    return (DataWriter<T>)this.topicWriterCollection[topicName];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// DDS 참여자가 보유한 모든 배포권을 해제합니다.
        /// </summary>
        public void clearPublication()
        {
            lock (this.topicWriterCollection)
            {
                foreach (AnyDataWriter topicWriter in this.topicWriterCollection.Values)
                {
                    topicWriter.Dispose();
                }

                this.topicWriterCollection.Clear();
            }
        }


        /// <summary>
        /// DDS 참여자가 특정 Topic명으로 구독권을 보유하고 있는지 확인합니다.
        /// </summary>
        /// <param name="topicName">Topic명</param>
        /// <returns>DDS 참여자가 Topic의 구독권을 보유하고 있는 경우 true, 그렇지 않으면 false를 반환합니다.</returns>
        public bool isSubscriptionTopic(string topicName)
        {
            lock (this.topicReaderResourceCollection)
            {
                return this.topicReaderResourceCollection.ContainsKey(topicName);
            }
        }

        /// <summary>
        /// DDS 참여자가 특정 Topic 클래스로 구독권을 보유하고 있는지 확인합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <returns>DDS 참여자가 Topic의 구독권을 보유하고 있는 경우 true, 그렇지 않으면 false를 반환합니다.</returns>
        public bool isSubscriptionTopic<T>() where T : IEquatable<T>
        {
            lock (this.topicReaderResourceCollection)
            {
                string topicName = getTopicName<T>();

                return this.topicReaderResourceCollection.ContainsKey(topicName);
            }
        }

        /// <summary>
        /// [사용 주의] 반환된 DataReader는 DDS 참여자에 의해 관리되고 있으므로, 외부에서 직접 사용하는 경우 Dispose와 관련된 문제를 유발할 수 있습니다.
        /// <br/>
        /// Topic 클래스에 대한 DataReader 객체를 반환합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <returns>DDS 참여자가 DataReader를 보유하고 있지 않으면 null을 반환합니다.</returns>
        public DataReader<T>? getTopicReader<T>() where T : IEquatable<T>
        {
            lock (this.topicReaderResourceCollection)
            {
                string topicName = getTopicName<T>();

                if (this.topicReaderResourceCollection.ContainsKey(topicName))
                {
                    return (DataReader<T>)this.topicReaderResourceCollection[topicName].dataReader;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// DDS 참여자가 보유한 모든 구독권을 해제합니다.
        /// </summary>
        public void clearSubscription()
        {
            lock (this.topicReaderResourceCollection)
            {
                foreach (AnyDataReaderResource topicReaderResource in this.topicReaderResourceCollection.Values)
                {
                    this.topicWaiter.DetachCondition(topicReaderResource.dataReader.StatusCondition);
                    topicReaderResource.Dispose();
                }

                this.topicReaderResourceCollection.Clear();
            }
        }


        /// <summary>
        /// DDS 참여자가 보유한 모든 배포/구독권을 해제하고, Topic Entity 객체를 정리합니다.
        /// </summary>
        public void clearAll()
        {
            clearPublication();
            clearSubscription();
            clearTopicEntityCollection();
        }


        /// <summary>
        /// DDS 참여자를 통해 Topic 메시지를 전송합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="topic">DDS Topic 메시지</param>
        /// <returns>DDS 참여자가 Topic에 대한 배포권이 없는 경우 false를 반환합니다.</returns>
        public bool writeTopic<T>(T topic) where T : IEquatable<T>
        {
            lock (this.topicWriterCollection)
            {
                string topicName = getTopicName<T>();

                if (this.topicWriterCollection.ContainsKey(topicName))
                {
                    DataWriter<T> writer = (DataWriter<T>)this.topicWriterCollection[topicName];
                    writer.Write(topic);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// DDS 참여자에게 특정 Topic 클래스에 대한 배포권을 부여합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        public void publishTopic<T>() where T : IEquatable<T>
        {
            publishTopic<T>($"PA::{getTopicName<T>()}");
        }

        /// <summary>
        /// DDS 참여자에게 특정 Topic 클래스에 대한 배포권을 부여합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="qosProfileName">QoS Profile명</param>
        public void publishTopic<T>(string qosProfileName) where T : IEquatable<T>
        {
            lock (this.topicWriterCollection)
            {
                string topicName = getTopicName<T>();

                if (!this.topicWriterCollection.ContainsKey(topicName))
                {
                    this.topicWriterCollection.Add(topicName, createTopicWriter<T>(qosProfileName));
                }
            }
        }

        /// <summary>
        /// DDS 참여자에게 특정 Topic 클래스에 대한 배포권을 부여합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="qos">DDS DataWriter QoS</param>
        public void publishTopic<T>(DataWriterQos qos) where T : IEquatable<T>
        {
            lock (this.topicWriterCollection)
            {
                string topicName = getTopicName<T>();

                if (!this.topicWriterCollection.ContainsKey(topicName))
                {
                    this.topicWriterCollection.Add(topicName, createTopicWriter<T>(qos));
                }
            }
        }

        /// <summary>
        /// DDS 참여자의 특정 Topic 클래스에 대한 배포권을 해제합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        public void unpublishTopic<T>() where T : IEquatable<T>
        {
            lock (this.topicWriterCollection)
            {
                string topicName = getTopicName<T>();

                if (this.topicWriterCollection.ContainsKey(topicName))
                {
                    DataWriter<T> topicWriter = (DataWriter<T>)this.topicWriterCollection[topicName];
                    topicWriter.Dispose();

                    this.topicWriterCollection.Remove(topicName);
                }
            }
        }

        /// <summary>
        /// [사용 주의] 외부에서 직접 사용하는 경우, DDS 참여자의 DataWriter 관리 체계를 벗어나는 호출이므로 배포와 관련된 문제를 유발할 수 있습니다.
        /// <br/>
        /// Topic 클래스에 대한 DataWriter 객체를 생성합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="qosProfileName">QoS Profile명</param>
        /// <returns></returns>
        public DataWriter<T> createTopicWriter<T>(string qosProfileName) where T : IEquatable<T>
        {
            this.publisher ??= this.substance.CreatePublisher();

            Topic<T> topic = getTopicEntity<T>();
            DataWriter<T> topicWriter;

            try
            {
                DataWriterQos topicWriterQos = QosProvider.Default.GetDataWriterQos(qosProfileName); // 없을 때 default 쓰게 하는 건 try catch로 처리할 수밖에 없을 듯?
                topicWriter = this.publisher.CreateDataWriter(topic, topicWriterQos);
            }
            catch (Exception)
            {
                topicWriter = this.publisher.CreateDataWriter(topic);
            }

            return topicWriter;
        }

        /// <summary>
        /// [사용 주의] 외부에서 직접 사용하는 경우, DDS 참여자의 DataWriter 관리 체계를 벗어나는 호출이므로 배포와 관련된 문제를 유발할 수 있습니다.
        /// <br/>
        /// Topic 클래스에 대한 DataWriter 객체를 생성합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="qos">DDS DataWriter QoS</param>
        /// <returns></returns>
        public DataWriter<T> createTopicWriter<T>(DataWriterQos qos) where T : IEquatable<T>
        {
            this.publisher ??= this.substance.CreatePublisher();

            Topic<T> topic = getTopicEntity<T>();
            DataWriter<T> topicWriter = this.publisher.CreateDataWriter(topic, qos);

            return topicWriter;
        }


        /// <summary>
        /// DDS 참여자에게 특정 Topic 클래스에 대한 구독권을 부여합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="receiveAction">Receive 콜백</param>
        /// <param name="enabledStatuses">Enabled Statuses</param>
        public void subscribeTopic<T>(Action<LoanedSample<T>> receiveAction, StatusMask enabledStatuses = StatusMask.DataAvailable) where T : IEquatable<T>
        {
            subscribeTopic<T>($"PA::{getTopicName<T>()}", receiveAction, enabledStatuses);
        }

        /// <summary>
        /// DDS 참여자에게 특정 Topic 클래스에 대한 구독권을 부여합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="receiveAction">Receive 콜백</param>
        /// <param name="enabledStatuses">Enabled Statuses</param>
        public void subscribeTopic<T>(Action<List<LoanedSample<T>>> receiveAction, StatusMask enabledStatuses = StatusMask.DataAvailable) where T : IEquatable<T>
        {
            subscribeTopic<T>($"PA::{getTopicName<T>()}", receiveAction, enabledStatuses);
        }

        /// <summary>
        /// DDS 참여자에게 특정 Topic 클래스에 대한 구독권을 부여합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="qosProfileName">QoS Profile명</param>
        /// <param name="receiveAction">Receive 콜백</param>
        /// <param name="enabledStatuses">Enabled Statuses</param>
        public void subscribeTopic<T>(string qosProfileName, Action<LoanedSample<T>> receiveAction, StatusMask enabledStatuses = StatusMask.DataAvailable) where T : IEquatable<T>
        {
            lock (this.topicReaderResourceCollection)
            {
                string topicName = getTopicName<T>();

                if (!this.topicReaderResourceCollection.ContainsKey(topicName))
                {
                    DataReader<T> topicReader = createTopicReader<T>(qosProfileName);
                    AnyDataReaderResource topicReaderResource = new AnyDataReaderResource(topicReader,
                                                                                          (condition) => invokeReading(topicReader, receiveAction),
                                                                                          enabledStatuses);

                    this.topicReaderResourceCollection.Add(topicName, topicReaderResource);
                    this.topicWaiter.AttachCondition(topicReader.StatusCondition);

                    initializeTopicWaiter();
                }
            }
        }

        /// <summary>
        /// DDS 참여자에게 특정 Topic 클래스에 대한 구독권을 부여합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="qosProfileName">QoS Profile명</param>
        /// <param name="receiveAction">Receive 콜백</param>
        /// <param name="enabledStatuses">Enabled Statuses</param>
        public void subscribeTopic<T>(string qosProfileName, Action<List<LoanedSample<T>>> receiveAction, StatusMask enabledStatuses = StatusMask.DataAvailable) where T : IEquatable<T>
        {
            lock (this.topicReaderResourceCollection)
            {
                string topicName = getTopicName<T>();

                if (!this.topicReaderResourceCollection.ContainsKey(topicName))
                {
                    DataReader<T> topicReader = createTopicReader<T>(qosProfileName);
                    AnyDataReaderResource topicReaderResource = new AnyDataReaderResource(topicReader,
                                                                                          (condition) => invokeReading(topicReader, receiveAction),
                                                                                          enabledStatuses);

                    this.topicReaderResourceCollection.Add(topicName, topicReaderResource);
                    this.topicWaiter.AttachCondition(topicReader.StatusCondition);

                    initializeTopicWaiter();
                }
            }
        }

        /// <summary>
        /// DDS 참여자에게 특정 Topic 클래스에 대한 구독권을 부여합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="qos">DDS DataReader QoS</param>
        /// <param name="receiveAction">Receive 콜백</param>
        /// <param name="enabledStatuses">Enabled Statuses</param>
        public void subscribeTopic<T>(DataReaderQos qos, Action<LoanedSample<T>> receiveAction, StatusMask enabledStatuses = StatusMask.DataAvailable) where T : IEquatable<T>
        {
            lock (this.topicReaderResourceCollection)
            {
                string topicName = getTopicName<T>();

                if (!this.topicReaderResourceCollection.ContainsKey(topicName))
                {
                    DataReader<T> topicReader = createTopicReader<T>(qos);
                    AnyDataReaderResource topicReaderResource = new AnyDataReaderResource(topicReader,
                                                                                          (condition) => invokeReading(topicReader, receiveAction),
                                                                                          enabledStatuses);

                    this.topicReaderResourceCollection.Add(topicName, topicReaderResource);
                    this.topicWaiter.AttachCondition(topicReader.StatusCondition);

                    initializeTopicWaiter();
                }
            }
        }

        /// <summary>
        /// DDS 참여자에게 특정 Topic 클래스에 대한 구독권을 부여합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="qos">DDS DataReader QoS</param>
        /// <param name="receiveAction">Receive 콜백</param>
        /// <param name="enabledStatuses">Enabled Statuses</param>
        public void subscribeTopic<T>(DataReaderQos qos, Action<List<LoanedSample<T>>> receiveAction, StatusMask enabledStatuses = StatusMask.DataAvailable) where T : IEquatable<T>
        {
            lock (this.topicReaderResourceCollection)
            {
                string topicName = getTopicName<T>();

                if (!this.topicReaderResourceCollection.ContainsKey(topicName))
                {
                    DataReader<T> topicReader = createTopicReader<T>(qos);
                    AnyDataReaderResource topicReaderResource = new AnyDataReaderResource(topicReader,
                                                                                          (condition) => invokeReading(topicReader, receiveAction),
                                                                                          enabledStatuses);

                    this.topicReaderResourceCollection.Add(topicName, topicReaderResource);
                    this.topicWaiter.AttachCondition(topicReader.StatusCondition);

                    initializeTopicWaiter();
                }
            }
        }

        /// <summary>
        /// DDS 참여자의 특정 Topic 클래스에 대한 구독권을 해제합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        public void unsubscribeTopic<T>() where T : IEquatable<T>
        {
            lock (this.topicReaderResourceCollection)
            {
                string topicName = getTopicName<T>();

                if (this.topicReaderResourceCollection.ContainsKey(topicName))
                {
                    AnyDataReaderResource topicReaderResource = this.topicReaderResourceCollection[topicName];
                    this.topicWaiter.DetachCondition(topicReaderResource.dataReader.StatusCondition);
                    topicReaderResource.Dispose();

                    this.topicReaderResourceCollection.Remove(topicName);
                }

                if (this.topicReaderResourceCollection.Count < 1)
                {
                    destroyTopicWaiter();
                }
            }
        }

        /// <summary>
        /// [사용 주의] 외부에서 직접 사용하는 경우, DDS 참여자의 DataReader 관리 체계를 벗어나는 호출이므로 구독과 관련된 문제를 유발할 수 있습니다.
        /// <br/>
        /// Topic 클래스에 대한 DataReader 객체를 생성합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="qosProfileName">QoS Profile명</param>
        /// <returns></returns>
        public DataReader<T> createTopicReader<T>(string qosProfileName) where T : IEquatable<T>
        {
            this.subscriber ??= this.substance.CreateSubscriber();

            Topic<T> topic = getTopicEntity<T>();
            DataReader<T> topicReader;

            try
            {
                DataReaderQos topicReaderQos = QosProvider.Default.GetDataReaderQos(qosProfileName);
                topicReader = this.subscriber.CreateDataReader(topic, topicReaderQos);
            }
            catch (Exception)
            {
                topicReader = this.subscriber.CreateDataReader(topic);
            }

            return topicReader;
        }

        /// <summary>
        /// [사용 주의] 외부에서 직접 사용하는 경우, DDS 참여자의 DataReader 관리 체계를 벗어나는 호출이므로 구독과 관련된 문제를 유발할 수 있습니다.
        /// <br/>
        /// Topic 클래스에 대한 DataReader 객체를 생성합니다.
        /// </summary>
        /// <typeparam name="T">Auto Generated 된 Topic 클래스 타입</typeparam>
        /// <param name="qos">DDS DataReader QoS</param>
        /// <returns></returns>
        public DataReader<T> createTopicReader<T>(DataReaderQos qos) where T : IEquatable<T>
        {
            this.subscriber ??= this.substance.CreateSubscriber();

            Topic<T> topic = getTopicEntity<T>();
            DataReader<T> topicReader = this.subscriber.CreateDataReader(topic, qos);

            return topicReader;
        }

        private void invokeReading<T>(DataReader<T> topicReader, Action<LoanedSample<T>> receiveAction)
        {
            lock (topicReader)
            {
                if (!topicReader.Disposed)
                {
                    using (LoanedSamples<T> topicPayloads = topicReader.Take())
                    {
                        foreach (LoanedSample<T> topicPayload in topicPayloads)
                        {
                            receiveAction.Invoke(topicPayload);
                        }
                    }
                }
            }
        }

        private void invokeReading<T>(DataReader<T> topicReader, Action<List<LoanedSample<T>>> receiveAction)
        {
            lock (topicReader)
            {
                if (!topicReader.Disposed)
                {
                    using (LoanedSamples<T> topicPayloads = topicReader.Take())
                    {
                        receiveAction.Invoke(topicPayloads.ToList());
                    }
                }
            }
        }


        private void initializeTopicWaiter()
        {
            lock (this.topicWaiter)
            {
                if (this.topicWaiterThread == null) // task 방식이라도 거의 재활용을 못하는데 (어떤 방식으로 하든 큰 의미가 없는 것 같음) 
                {
                    this.waitingTopic = true;

                    this.topicWaiterThread = new Thread(() => {
                        while (this.waitingTopic)
                        {
                            this.topicWaiter.Dispatch(this.topicWaiterDuration); // 그냥 topicReader.Take() 이걸 걸어두면 안 되나? 왜 condition trigger로 read를 해야하는 거지? (만약에 reader 별로 걸어두면 구독한 topic 갯수만큼 thread 같은 것을 만들어야 하니까?)
                        } // topicWaiterThread 쉽게 정리하려면 Disptch를 infinite로 안 하는 수밖에? (waiter를 dispose 시켜도 error를 발생시키지 않아서 infinite dispatch 걸어둔 그대로 붙잡혀 있음)
                    }) { IsBackground = true, };
                    this.topicWaiterThread.Start();
                }
            }
        }

        private void destroyTopicWaiter()
        {
            lock (this.topicWaiter)
            {
                this.waitingTopic = false;

                this.topicWaiterThread?.Join();
                this.topicWaiterThread = null;
            }
        }
    }
}