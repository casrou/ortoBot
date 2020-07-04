# ortoBot [![Build Status](https://travis-ci.com/casrou/ortoBot.svg?token=9rfyGsm1xydBPACurxpn&branch=master)](https://travis-ci.com/casrou/ortoBot)

Let your Nanoleaf lights be controlled by Twitch subscriptions, bit-donations and more! Built in collaboration with the one and only [ortopilot](https://www.twitch.tv/ortopilot).

![Demo](https://github.com/casrou/ortobot/raw/master/media/demo.gif "Demo")
## Getting Started
_work in progress.._
### Appsettings
- Copy `ortoBot/appsettings.example.json` and rename to `ortoBot/appsettings.json`
- Change the following:
```
debug: true
channelId: not necessary (twitch pubsub)
oauth: not necessary (twitch pubsub)
ip: not necessary (ip of nanoleaf lights)
authToken: not necessary (auth token of nanoleaf lights)
botUsername: your twitch username, eg. "casrou"
botOauth: use ACCESS TOKEN from below, ie. "oauth:*ACCESS TOKEN*" (without asterisks)
botJoinChannel: your twitch username, eg. "casrou"
```
### Get OAuth
- https://twitchtokengenerator.com/
- Bot Chat Token
- Confirm
- I'm not a robot
- Copy ACCESS TOKEN

## Testing
#### Visual Studio
- Start debugging
- Go to your Twitch channel (you don't have to be streaming)
- Try a bot command in the chat, e.g. "!ortobot effects"

![The output of the command should then be printed to the console](https://github.com/casrou/ortobot/raw/master/media/gettingstarted1.png "Getting Started")
