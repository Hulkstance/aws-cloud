using System.Collections.Generic;
using System.Text.Json;
using Pulumi;
using Aws = Pulumi.Aws;

return await Deployment.RunAsync(async () =>
{
    var currentCallerIdentity = await Aws.GetCallerIdentity.InvokeAsync();
    var currentRegion = await Aws.GetRegion.InvokeAsync();
    var accountId = currentCallerIdentity.AccountId;
    var region = currentRegion.Name;

    // SQS
    var queue = new Aws.Sqs.Queue("queue", new()
    {
        Name = "customers"
    });

    // SNS
    var topic = new Aws.Sns.Topic("topic", new()
    {
        Name = "customers"
    });

    // SNS Topic Subscription
    var subscription = new Aws.Sns.TopicSubscription("topic-subscription", new()
    {
        Topic = topic.Arn,
        Protocol = "sqs",
        Endpoint = queue.Arn,
        RawMessageDelivery = true
    });

    // Allow SNS topic to publish messages to SQS queue
    var queuePolicy = queue.Arn.Apply(arn => Aws.Iam.GetPolicyDocument.Invoke(new Aws.Iam.GetPolicyDocumentInvokeArgs
    {
        Version = "2012-10-17",
        PolicyId = "__default_policy_ID",
        Statements = new Aws.Iam.Inputs.GetPolicyDocumentStatementInputArgs
        {
            Effect = "Allow",
            Principals = new Aws.Iam.Inputs.GetPolicyDocumentStatementPrincipalInputArgs
            {
                Type = "Service",
                Identifiers = "sns.amazonaws.com",
            },
            Actions = "SQS:SendMessage",
            Resources = arn,
            Conditions = new Aws.Iam.Inputs.GetPolicyDocumentStatementConditionInputArgs
            {
                Test = "ArnEquals",
                Variable = "AWS:SourceArn",
                Values = { topic.Arn }
            }
        }
    }));

    var sqsQueuePolicy = new Aws.Sqs.QueuePolicy("queue-policy", new Aws.Sqs.QueuePolicyArgs
    {
        QueueUrl = queue.Url,
        Policy = queuePolicy.Apply(policy => policy.Json)
    });

    // SQS Dead Letter Queue (DLQ)
    var deadLetterQueue = new Aws.Sqs.Queue("dead-letter-queue", new()
    {
        Name = "customers-dlq",
        MessageRetentionSeconds = 1209600 // 14 days
    });

    var redrivePolicy = new Aws.Sqs.RedrivePolicy("redrive-policy", new()
    {
        QueueUrl = queue.Url,
        RedrivePolicyName = deadLetterQueue.Arn.Apply(arn => JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["deadLetterTargetArn"] = arn,
            ["maxReceiveCount"] = 3
        }))
    });

    // Export stuff
    return new Dictionary<string, object?>
    {
        ["accountId"] = accountId,
        ["region"] = region,
        ["queueUrl"] = queue.Url,
        ["queueArn"] = queue.Arn,
        ["topicArn"] = topic.Arn,
        ["subscriptionName"] = subscription.Id,
        ["deadLetterQueueUrl"] = deadLetterQueue.Url,
        ["deadLetterQueueArn"] = deadLetterQueue.Arn,
    };
});
