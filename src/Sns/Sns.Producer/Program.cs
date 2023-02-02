using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Sns.Contracts;

var customer = new CustomerCreated
{
    Id = Guid.NewGuid(),
    Email = "johnsmith@gmail.com",
    FullName = "John Smith",
    DateOfBirth = new DateTime(1990, 12, 12),
    GitHubUsername = "johnsmith"
};

var snsClient = new AmazonSimpleNotificationServiceClient();

var topicArnResponse = await snsClient.FindTopicAsync("customers");

var publishRequest = new PublishRequest
{
    TopicArn = topicArnResponse.TopicArn,
    Message = JsonSerializer.Serialize(customer),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        { "MessageType", new MessageAttributeValue { DataType = "String", StringValue = nameof(CustomerCreated) } }
    }
};

var response = await snsClient.PublishAsync(publishRequest);
