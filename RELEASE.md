# Releasing

- Update card images in repo
- Update version number in `package.json`
  - Bump major version when the JSON format changes in a breaking way
  - Bump minor version when new cards are added
  - Bump patch version when card images are updated
- Update `images.json`: `make update-images`
- Commit: `git commit`
- Tag release: `git tag -s x.y.z -m 'Release x.y.z.'`
- Push changes: `git push && git push --tags`
- Publish package: `npm publish`
