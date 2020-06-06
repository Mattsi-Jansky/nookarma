# Nookarma

## About

A karma middleware for the Slack bot framework Noobot. Do you remember karma bots from back in the IRC days? They worked like this:

```
user: Gosh, I sure don't like rainy days. Rainydays--
bot: Rainydays - 1, at -1
user2: Yeah! sunnydays++ for being sunny
bot: sunnydays + 1 for being sunny, at 2
```

Well, Nookarma does that for Slack:

![A screenshot showing a Slack user saying "Gosh, I sure don't like rainy days. Rainydays-- but sunnydays++ for being sunny". A bot then responds with an emoji of a down arrow saying "Rainydays: -1" and an emoji of an up arrow saying "sunnydays: 1 for being sunny"](example.png)

## Dependencies

* Dotnet core SDK: https://dotnet.microsoft.com/download

## How to run the tests

* `dotnet test`