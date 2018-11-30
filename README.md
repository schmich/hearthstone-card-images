# Hearthstone Card Images [![npm](https://img.shields.io/npm/v/hearthstone-card-images.svg)](https://www.npmjs.com/package/hearthstone-card-images)

Hearthstone card image repository for use in conjunction with [HearthstoneJSON](https://hearthstonejson.com/) data. Try [the demo application](https://schmich.github.io/hearthstone-card-images/). See also [related projects](#related-projects).

All card images and names copyright © Blizzard Entertainment, Inc. Hearthstone® is a registered trademark of Blizzard Entertainment, Inc. Hearthstone Card Images is not affiliated or associated with or endorsed by Hearthstone® or Blizzard Entertainment, Inc.

![](https://github.com/schmich/hearthstone-card-images/raw/master/rel/50443.png)
![](https://github.com/schmich/hearthstone-card-images/raw/master/rel/50012.png)

## Overview

- [HearthstoneJSON](https://hearthstonejson.com/) makes [Hearthstone card info available](https://api.hearthstonejson.com/v1/latest/enUS/cards.json) with unique DBF IDs for each card
- Download and cache card images from this repo using one of the methods below
- Use a card's DBF ID to serve its image from your site or app

## Downloading Images

### Via compressed archive

This method is best if you wish to use the provided [`sync.sh`](sync.sh) script to stay up-to-date.

Setup: Download card images as a .zip or .tar.gz compressed archive from the [releases page](https://github.com/schmich/hearthstone-card-images/releases). Extract and combine card images in the `pre` (prerelease cards) and `rel` (released cards) folders into a single card images folder.

Updating: Download and run the [`sync.sh`](sync.sh) script to keep your images up-to-date with the latest images in this repo. 

```bash
curl -LOs https://raw.githubusercontent.com/schmich/hearthstone-card-images/master/sync.sh
# Inspect sync.sh.
bash sync.sh images
```

[`sync.sh`](sync.sh) incrementally downloads card images from the repo to a local directory if the card image has changed or is missing locally. It compares the local file hash to the card image hash stored in [`images.json`](images.json). The hash is the 5 character prefix of the base64-encoded SHA1 hash of the image contents.

### Via git

This method is best if you wish to use `git` to stay up-to-date.

Setup: Create a shallow clone of this repo unless you absolutely need the full history. There are a lot of outdated card images that you can avoid altogether if you don't need them.

```bash
git clone --depth 1 --branch master --single-branch https://github.com/schmich/hearthstone-card-images
```

Updating: Use `git fetch` and `git reset` to [update your shallow clone](https://stackoverflow.com/a/41081908) without growing your local git history.

```bash
git fetch --depth 1 && git reset --hard origin/master
```

### Via images.json

This method is best if you want fine-grained control over downloading and updating images.

You can use [`images.json`](images.json) to fetch card image URLs by DBF ID. It is published as a [versioned NPM package](https://www.npmjs.com/package/hearthstone-card-images) that gets updated when cards are added or changed (see [versioning](#versioning)).

Example: Using JavaScript to map from a card's DBF ID to its card image URL using [`images.json`](images.json):

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

## Related Projects

- [HearthstoneJSON](https://github.com/HearthSim/hearthstonejson): Hearthstone datamining and API scripts
- [Sunwell](https://github.com/HearthSim/Sunwell): Canvas-based high quality Hearthstone card renderer
- [Hearthstone Cards for WordPress](https://github.com/flowdee/hearthstone-cards): WordPress plugin to display Hearthstone cards
- [HearthCards.net](http://hearthcards.net/): Hearthstone card generator

## License

All card images and names copyright © Blizzard Entertainment, Inc. Hearthstone® is a registered trademark of Blizzard Entertainment, Inc. Hearthstone Card Images is not affiliated or associated with or endorsed by Hearthstone® or Blizzard Entertainment, Inc.

All else copyright © 2016 Chris Schmich  
MIT License. See [LICENSE](LICENSE) for details.
