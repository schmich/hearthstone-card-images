# Releasing

- Update card images in repo.
- Commit: `git commit`
- Run `ruby create-images.rb` to update `images.json`.
- Update version number in `package.json` (only bump major version if format changes).
- Commit: `git commit`
- Tag release: `git tag -s x.y.z -m 'Release x.y.z.'`
- Push changes: `git push`
- Publish package: `npm publish`
