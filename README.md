# Hearthstone Card Images [![npm](https://img.shields.io/npm/v/hearthstone-card-images.svg)](https://www.npmjs.com/package/hearthstone-card-images)

Hearthstone card image repository for use in conjunction with [HearthstoneJSON](https://hearthstonejson.com/) data.

![](https://github.com/schmich/hearthstone-card-images/raw/master/rel/42818.png)
![](https://github.com/schmich/hearthstone-card-images/raw/master/rel/42759.png)

## Usage

- [HearthstoneJSON](https://hearthstonejson.com/) makes Hearthstone card info available in [JSON format](https://api.hearthstonejson.com/v1/latest/enUS/cards.json)
- Use [`images.json`](images.json) to map from a Hearthstone card ID to a repository URL for that card's image
- Download, cache, and use card images on your own server or in your own app

## Updating Images

You can periodically fetch [`images.json`](images.json) and update your images using the URLs there. The image map is also published as a [versioned NPM package](https://www.npmjs.com/package/hearthstone-card-images):

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

- The image URL is a GitHub asset URL and *must not* be used as a CDN
- You should download, cache, and serve images yourself
- Released cards are under the [`rel`](rel) folder; prerelease cards are under the [`pre`](pre) folder
- For prerelease cards, their name serves as their card ID until they are released
- Not all cards in HearthstoneJSON's [`cards.json`](https://api.hearthstonejson.com/v1/latest/enUS/cards.json) have a corresponding image
- Currently, only en-US card images are available

## License

All card images and names copyright © Blizzard Entertainment, Inc. Hearthstone® is a registered trademark of Blizzard Entertainment, Inc. Hearthstone Card Images is not affiliated or associated with or endorsed by Hearthstone® or Blizzard Entertainment, Inc.

All else copyright © 2016 Chris Schmich  
MIT License. See [LICENSE](LICENSE) for details.
