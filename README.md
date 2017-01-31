# Hearthstone Card Images

Hearthstone card images served via CDN for use in conjunction with [HearthstoneJSON](https://hearthstonejson.com/) data.

## Usage

- [HearthstoneJSON](https://hearthstonejson.com/) makes Hearthstone card info available in [JSON format](https://api.hearthstonejson.com/v1/latest/enUS/cards.json)
- Use [`map.json`](map.json) to map from a card ID to a CDN URL for that card's image
- The CDN URL points to a permanent, cached image
- [`map.json`](map.json) is updated with a new CDN URL when a card's image changes

## Notes

- Released cards are under the [`rel`](rel) folder; pre-release cards are under the [`pre`](pre) folder
- Not all cards in HearthstoneJSON's [`cards.json`](https://api.hearthstonejson.com/v1/latest/enUS/cards.json) have a corresponding image
- Currently, only en-US card images are available
- CDN graciously provided by [RawGit](http://rawgit.com/) and [MaxCDN](http://www.maxcdn.com/)

## License

All images and names copyright © Blizzard Entertainment, Inc.

All else copyright © 2016 Chris Schmich  
MIT License. See [LICENSE](LICENSE) for details.
