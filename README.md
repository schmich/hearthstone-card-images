# Hearthstone Card Images [![npm](https://img.shields.io/npm/v/hearthstone-card-images.svg)](https://www.npmjs.com/package/hearthstone-card-images)

Hearthstone card images for all locales for use in conjunction with the [official Hearthstone API](https://develop.battle.net/documentation/api-reference/hearthstone-game-data-api) or [HearthstoneJSON data](https://hearthstonejson.com/).

Try [the demo application](https://schmich.github.io/hearthstone-card-images/) or see [related projects](#related-projects).

All card images and names copyright © Blizzard Entertainment, Inc. Hearthstone® is a registered trademark of Blizzard Entertainment, Inc. Hearthstone Card Images is not affiliated or associated with or endorsed by Hearthstone® or Blizzard Entertainment, Inc.

![](https://github.com/schmich/hearthstone-card-images/raw/master/cards/en_US/53551.png)
![](https://github.com/schmich/hearthstone-card-images/raw/master/cards/en_US/54261.png)

## Overview

- The [official Hearthstone API](https://develop.battle.net/documentation/api-reference/hearthstone-game-data-api) and [HearthstoneJSON](https://hearthstonejson.com/) make Hearthstone JSON card info available
- Each card has a DBF ID that uniquely identifies it
- Use this repo to download and cache card images using one of the methods below
- Use a card's DBF ID to serve its image from your site or app

## Downloading Images

### Option 1: Using git

This method is best if you wish to use `git` to stay up-to-date.

Setup: Create a shallow clone of this repo unless you absolutely need the full history. There are a lot of outdated card images that you can avoid altogether if you don't need them.

```bash
git clone --depth 1 --branch master --single-branch https://github.com/schmich/hearthstone-card-images
```

Updating: [Update your shallow clone](https://stackoverflow.com/a/41081908) without growing your local git history with the following:

```bash
git fetch --depth 1 && git reset --hard origin/master
```

### Option 2: Using JSON manifests

This method is best if you want fine-grained control over downloading and updating images.

Setup: Download the card images as a .zip or .tar.gz compressed archive from the [releases page](https://github.com/schmich/hearthstone-card-images/releases).

Updating: Use the [versioned NPM package](https://www.npmjs.com/package/hearthstone-card-images) to track MD5 hashes of each card image for each locale. The package is updated when cards are added, removed, or updated (see [versioning](#versioning)).

The package entrypoint points to [`manifest/all.json`](manifest/all.json) which, for each locale, maps each card DBF ID to the MD5 hash of its image contents. This hash can be used to determine if your local copy is up-to-date.

Example:

`npm install hearthstone-card-images`

```js
function* cardImageHashes() {
    const manifest = require('hearthstone-card-images');

    let base = manifest.config.base;
    let version = manifest.config.version;

    for (let locale in manifest.cards) {
        for (let dbfId in manifest.cards[locale]) {
            let url = `${base}/${version}/cards/${locale}/${dbfId}.png`;
            let hash = manifest.cards[locale][dbfId];
            yield { locale, dbfId, url, hash };
        }
    }
}

for (let card of cardImageHashes()) {
    // Properties:
    //   card.locale => en_US, pt_BR
    //   card.dbfId => 64 (Swipe)
    //   card.url => https://raw.githubusercontent.com/schmich/hearthstone-card-images/5.0.0/cards/en_US/64.png
    //   card.hash => ef4e301fff3ed65b9c21194e8b22d06c (MD5 hash of image contents)
    // Updating:
    //   1. Hash local file using MD5, card.locale, and card.dbfId
    //   2. Compare local file hash to card.hash
    //   3. If missing or different, download card.url locally
}
```

## Image Size Optimization

By default, images are 375x518 PNG format weighing around 100 KB each. A few things can be done to reduce the space and bandwidth needed for storage and transmission:

### 1. Resize the images

If you don't need large images, you can resize them using [ImageMagick](https://imagemagick.org/index.php) or many other image processing toolkits.

`magick convert 64.png -resize 50% out.png`

### 2. Use a PNG optimizer

[pngquant](https://pngquant.org/) and [pngcrush](https://pmt.sourceforge.io/pngcrush/) optimize PNGs by removing extraneous metadata, trying multiple compression methods, and reducing the image's color palette by introducing dithering. Depending on your needs, the quality can be scaled to control the resulting file size.

`pngquant 64.png --quality 25 --output out.png`

### 3. Convert the images to JPG

If you don't need transparency, you can convert the images to JPG to save significantly on space. You can also trade off between quality and file size depending on your needs.

`magick convert 64.png -background white -flatten -quality 50% out.jpg`

### 4. Convert the images to WebP

The [WebP](https://en.wikipedia.org/wiki/WebP) image format has the benefits of JPG compression while also retaining the alpha transparency of PNG. However, this format [is not yet supported by all major browsers](https://caniuse.com/#search=webp). WebP has both lossless and lossy modes.

`magick convert 64.png -define webp:lossless=true out.webp`

`magick convert 64.png -quality 75% out.webp`

## Notes

- Card images are under the [`cards`](cards) folder
- Images are available for all Hearthstone locales
- The card image URL is a GitHub asset URL and *must not* be used as a CDN
- You *must* download, cache, and serve images yourself
- Not all cards with a DBF ID will necessarily have a corresponding image
- Currently, only non-golden card images are available
- Pre-release (spoilered) cards will also be under the `cards` folder since Blizzard now gives them a DBF ID when they are revealed

## Versioning

The NPM package adheres to [SemVer 2.0.0](http://semver.org/spec/v2.0.0.html):
- The major version changes when the JSON format changes in a breaking way
- The minor version changes when new cards are added
- The patch version changes when card images are updated

## Related Projects

- [Official Hearthstone API](https://develop.battle.net/documentation/api-reference/hearthstone-game-data-api): Blizzard's Hearthstone API
- [HearthstoneJSON](https://github.com/HearthSim/hearthstonejson): Hearthstone datamining and API scripts
- [Sunwell](https://github.com/HearthSim/Sunwell): Canvas-based high quality Hearthstone card renderer
- [Hearthstone Cards for WordPress](https://github.com/flowdee/hearthstone-cards): WordPress plugin to display Hearthstone cards
- [HearthCards.net](http://hearthcards.net/): Hearthstone card generator

## License

All card images and names copyright © Blizzard Entertainment, Inc. Hearthstone® is a registered trademark of Blizzard Entertainment, Inc. Hearthstone Card Images is not affiliated or associated with or endorsed by Hearthstone® or Blizzard Entertainment, Inc.

All else copyright © 2016 Chris Schmich  \
MIT License. See [LICENSE](LICENSE) for details.
