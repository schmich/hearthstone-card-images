# Hearthstone Card Images [![npm](https://img.shields.io/npm/v/hearthstone-card-images.svg)](https://www.npmjs.com/package/hearthstone-card-images)

Hearthstone card image repository for use in conjunction with [HearthstoneJSON](https://hearthstonejson.com/) data.

All card images and names copyright © Blizzard Entertainment, Inc. Hearthstone® is a registered trademark of Blizzard Entertainment, Inc. Hearthstone Card Images is not affiliated or associated with or endorsed by Hearthstone® or Blizzard Entertainment, Inc.

![](https://github.com/schmich/hearthstone-card-images/raw/master/rel/49246.png)
![](https://github.com/schmich/hearthstone-card-images/raw/master/rel/49251.png)

## Overview

- [HearthstoneJSON](https://hearthstonejson.com/) makes [Hearthstone card info available](https://api.hearthstonejson.com/v1/latest/enUS/cards.json) with unique DBF IDs for each card
- Download and cache card images from this repo using one of the methods below
- Use a card's DBF ID to serve its image from your site or app

## Downloading Images

### .zip or .tar.gz archive

This method is best if you have not yet downloaded any card images.

Card images can be downloaded as a compressed archive from the [releases page](https://github.com/schmich/hearthstone-card-images/releases). Download, extract, and combine card images in the `pre` (prerelease cards) and `rel` (released cards) folders into a single card image folder.

### sync.sh

This method is best if you want to update your existing card images to match the repo.

```bash
curl -LOs https://raw.githubusercontent.com/schmich/hearthstone-card-images/master/sync.sh
# Inspect sync.sh.
bash sync.sh images
```

[`sync.sh`](sync.sh) incrementally downloads card images from the repo to a local directory if the card image has changed or is missing. It compares the local file hash to the card image hash stored in [`images.json`](images.json). The hash is the 5 character prefix of the base64-encoded SHA1 hash of the image contents.

### images.json

This method is best if you want fine-grained control over downloading images.

You can use [`images.json`](images.json) to fetch card image URLs by DBF ID. [`images.json`](images.json) is published as a [versioned NPM package](https://www.npmjs.com/package/hearthstone-card-images):

`npm i hearthstone-card-images`

```js
// Build map from DBF ID to card image URL.
function indexCardImages() {
  const database = require('hearthstone-card-images');

  let images = {};
  let base = database.config.base;
  let version = database.config.version;
  for (let type in database.cards) {
    for (let id in database.cards[type]) {
      let url = `${base}/${version}/${type}/${id}.png`;
      images[id] = url;
    }
  }

  return images;
}

// Show all available card images.
let images = indexCardImages();
for (let id in images) {
  let url = images[id];
  console.log(`${id} -> ${url}`);
}
```

## Notes

- The card image URL is a GitHub asset URL and *must not* be used as a CDN
- You *must* download, cache, and serve images yourself
- Released cards are under the [`rel`](rel) folder; prerelease cards are under the [`pre`](pre) folder
- For prerelease cards, their name serves as their card ID until they are released
- Not all cards in HearthstoneJSON's [`cards.json`](https://api.hearthstonejson.com/v1/latest/enUS/cards.json) have a corresponding image
- Currently, only non-golden en-US card images are available

## Versioning

The NPM package adheres to [SemVer 2.0.0](http://semver.org/spec/v2.0.0.html):
- The major version changes when the JSON format changes in a breaking way
- The minor version changes when new cards are added
- The patch version changes when card images are updated

## License

All card images and names copyright © Blizzard Entertainment, Inc. Hearthstone® is a registered trademark of Blizzard Entertainment, Inc. Hearthstone Card Images is not affiliated or associated with or endorsed by Hearthstone® or Blizzard Entertainment, Inc.

All else copyright © 2016 Chris Schmich  
MIT License. See [LICENSE](LICENSE) for details.
