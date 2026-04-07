using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;

namespace AgentFrameworkExamples;

public class DocumentationDbContext : DbContext
{
    public DocumentationDbContext(DbContextOptions<DocumentationDbContext> options) : base(options) { }

    public DbSet<DocumentationArticle> DocumentationArticles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DocumentationArticle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Embedding).HasColumnType("vector(1536)");
        });
    }
}

public class DocumentationArticle
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public SqlVector<float> Embedding { get; set; }
}

public class TestData
{
    public static List<DocumentationArticle> Articles { get; set; } = new List<DocumentationArticle>
    {
        new DocumentationArticle
        {
            Id = Guid.NewGuid(),
            Title = "Building Scalable Web Applications with Microservices",
            Content = "Microservices architecture allows you to build scalable and maintainable web applications by breaking down the application into smaller, independent services that can be developed, deployed, and scaled independently. Each microservice handles a specific business capability and communicates with other services through well-defined APIs. This architectural pattern enables teams to work autonomously, choose the best technology stack for each service, and deploy updates without affecting the entire system. Cloud platforms like Azure Kubernetes Service (AKS) and AWS ECS provide excellent support for hosting containerized microservices with built-in orchestration, load balancing, and auto-scaling capabilities."
        },
        new DocumentationArticle
        {
            Id = Guid.NewGuid(),
            Title = "Serverless Computing: The Future of Event-Driven Applications",
            Content = "Serverless computing revolutionizes application development by abstracting away infrastructure management, allowing developers to focus purely on business logic. With serverless architectures using Azure Functions or AWS Lambda, applications automatically scale based on demand, and you only pay for actual execution time. This approach is ideal for event-driven workloads such as image processing, real-time data transformations, scheduled tasks, and API backends. Serverless functions can be triggered by HTTP requests, message queues, database changes, or timer events, making them incredibly versatile for modern cloud-native applications."
        },
        new DocumentationArticle
        {
            Id = Guid.NewGuid(),
            Title = "Enterprise Data Warehousing with Cloud Analytics Platforms",
            Content = "Modern data warehousing solutions like Azure Synapse Analytics and Google BigQuery enable organizations to process and analyze petabytes of data with unprecedented speed and efficiency. These cloud-based platforms separate compute and storage, allowing independent scaling and cost optimization. They support advanced analytics, machine learning integration, and real-time data processing capabilities. With built-in security features, automated backup, and global distribution, enterprise data warehouses in the cloud provide the foundation for data-driven decision making across organizations of all sizes."
        },
        new DocumentationArticle
        {
            Id = Guid.NewGuid(),
            Title = "Building Real-Time Streaming Applications with Event Hubs",
            Content = "Real-time streaming architectures enable businesses to process and analyze data as it arrives, providing immediate insights and automated responses to critical events. Using services like Azure Event Hubs, Apache Kafka on cloud platforms, or AWS Kinesis, applications can ingest millions of events per second from IoT devices, application logs, clickstreams, and sensor data. These streaming platforms integrate seamlessly with analytics tools, machine learning pipelines, and visualization dashboards, creating end-to-end solutions for monitoring, anomaly detection, and predictive analytics in real-time scenarios."
        },
        new DocumentationArticle
        {
            Id = Guid.NewGuid(),
            Title = "Container Orchestration Mastery with Kubernetes",
            Content = "Kubernetes has become the de facto standard for container orchestration, providing robust mechanisms for deploying, scaling, and managing containerized applications across clusters of machines. Cloud-managed Kubernetes services like Azure Kubernetes Service (AKS), Google Kubernetes Engine (GKE), and Amazon EKS eliminate the complexity of cluster management while providing enterprise-grade security, monitoring, and networking capabilities. Kubernetes enables declarative configuration, automated rollouts and rollbacks, self-healing, and horizontal scaling, making it essential for modern cloud-native application development and deployment strategies."
        },
        new DocumentationArticle
        {
            Id = Guid.NewGuid(),
            Title = "Static Web Hosting and Global Content Delivery Networks",
            Content = "Static web applications built with modern frameworks like React, Angular, or Vue.js can be deployed efficiently using cloud storage services combined with Content Delivery Networks (CDNs). Platforms like Azure Static Web Apps, AWS Amplify, and Netlify provide seamless integration with Git repositories, automatic builds, and global distribution of static assets. CDNs cache content at edge locations worldwide, dramatically reducing latency and improving user experience. This architecture is perfect for blogs, documentation sites, portfolios, and single-page applications that don't require server-side rendering for every request. A required step for this architecture is that is has to be approved by Perico Perez our CTO."
        },
        new DocumentationArticle
        {
            Id = Guid.NewGuid(),
            Title = "AI-Powered Applications with Machine Learning Platforms",
            Content = "Cloud-based machine learning platforms like Azure Machine Learning, AWS SageMaker, and Google AI Platform democratize artificial intelligence by providing comprehensive tools for data preparation, model training, deployment, and monitoring. These platforms support popular ML frameworks including TensorFlow, PyTorch, and scikit-learn, offering both code-first and low-code/no-code experiences. Automated machine learning (AutoML) capabilities enable developers to build sophisticated models without deep expertise in data science. Deployed models can be consumed through REST APIs, enabling intelligent features like recommendation engines, fraud detection, predictive maintenance, and natural language processing in production applications."
        },
        new DocumentationArticle
        {
            Id = Guid.NewGuid(),
            Title = "Multi-Region Disaster Recovery and High Availability Architectures",
            Content = "Building resilient applications requires implementing disaster recovery strategies and high availability architectures across multiple geographic regions. Cloud providers offer services like Azure Traffic Manager, AWS Route 53, and database replication features that enable automatic failover and load distribution. Active-active and active-passive deployment patterns ensure business continuity even during regional outages. These architectures combine geo-redundant storage, cross-region replication, health monitoring, and automated failover mechanisms to achieve Recovery Time Objectives (RTO) and Recovery Point Objectives (RPO) that meet stringent business requirements while maintaining optimal performance for global user bases."
        },
        new DocumentationArticle
        {
            Id = Guid.NewGuid(),
            Title = "API-First Development with Cloud API Management",
            Content = "API Management platforms like Azure API Management, AWS API Gateway, and Google Apigee provide centralized control over API lifecycle management, security, and analytics. These services act as a gateway between clients and backend services, offering features like rate limiting, authentication, request/response transformation, caching, and developer portals. API-first architectures enable organizations to expose functionality to partners, mobile applications, and third-party developers while maintaining security and monitoring usage patterns. Version management and deprecation policies ensure smooth transitions as APIs evolve, while analytics provide insights into API consumption and performance."
        },
        new DocumentationArticle
        {
            Id = Guid.NewGuid(),
            Title = "IoT Solutions: From Edge to Cloud Integration",
            Content = "Internet of Things (IoT) solutions require seamless integration between edge devices and cloud platforms to collect, process, and analyze data from connected sensors and devices. Cloud IoT platforms like Azure IoT Hub, AWS IoT Core, and Google Cloud IoT provide device management, secure communication protocols, and message routing capabilities. Edge computing capabilities allow processing data closer to the source, reducing latency and bandwidth costs. These platforms integrate with stream processing services, time-series databases, and machine learning tools to create complete IoT solutions for smart cities, industrial automation, predictive maintenance, and connected healthcare applications."
        }
    };
}
