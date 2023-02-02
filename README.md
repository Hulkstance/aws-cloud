## Root user

Regarding the root key, you should be using a key that is scoped to the services you will be working on during your dev time and ideally recreate it daily or weekly. In proper dev environments this process is automated. People use their SSO to automate the process, e.g. Okta. https://github.com/Nike-Inc/gimme-aws-creds

## AWS SQS

Standard can deliver messages out of order and may have duplicates, while FIFO SQS delivers messages in the order they were added and without duplicates.

## AWS SNS

If we go ahead and publish a message, the message goes nowhere if there are no subscriptions to catch it. The message is just lost.

`RawMessageDelivery` should be enabled because if we don't, SNS wraps our message into its own SNS event type which we don't need because we only care about the message itself.