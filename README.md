# Hearthstone Card Images [![npm](https://img.shields.io/npm/v/hearthstone-card-images.svg)](https://www.npmjs.com/package/hearthstone-card-images)

Hearthstone card images served via CDN for use in conjunction with [HearthstoneJSON](https://hearthstonejson.com/) data.

![](https://cdn.rawgit.com/schmich/hearthstone-card-images/7af5/rel/ICC_314.png)
![](https://cdn.rawgit.com/schmich/hearthstone-card-images/7af5/rel/ICC_281.png)

## Usage

- [HearthstoneJSON](https://hearthstonejson.com/) makes Hearthstone card info available in [JSON format](https://api.hearthstonejson.com/v1/latest/enUS/cards.json)
- Use [`images.json`](images.json) to map from a Hearthstone card ID to a CDN URL for that card's image

## Updating Images

You can periodically fetch [`images.json`](images.json) and cache the image URLs on your server or in your app. The image map is also published as a [versioned NPM package](https://www.npmjs.com/package/hearthstone-card-images):

`npm install --save hearthstone-card-images`

```js
const images = require('hearthstone-card-images');

for (const card of images) {
  console.log(`${card.id} -> ${card.url}`);
}
```

The package's version adheres to [SemVer 2.0.0](http://semver.org/spec/v2.0.0.html):
- The major version changes when the JSON format changes in a breaking way
- The minor version changes when new cards are added
- The patch version changes when card images are updated

## Notes

- The CDN URL points to a permanent, cached image
- [`images.json`](images.json) is updated with a new CDN URL when a card's image changes
- Released cards are under the [`rel`](rel) folder; prerelease cards are under the [`pre`](pre) folder
- For prerelease cards, their name serves as their card ID until they are released
- Not all cards in HearthstoneJSON's [`cards.json`](https://api.hearthstonejson.com/v1/latest/enUS/cards.json) have a corresponding image
- Currently, only en-US card images are available
- CDN graciously provided by [RawGit](http://rawgit.com/) and [StackPath](https://www.stackpath.com/)

## License

All card images and names copyright © Blizzard Entertainment, Inc.

All else copyright © 2016 Chris Schmich  
MIT License. See [LICENSE](LICENSE) for details.
