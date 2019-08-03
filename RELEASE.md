# Releasing

- Update version number in `package.json`
  - Bump major version when the JSON format changes in a breaking way
  - Bump minor version when new cards are added
  - Bump patch version when card images are updated
- Update card images and manifests
  - `make update-cards`
  - `make download-images`
  - `make copy-images`
  - `make check-images`
  - `make create-manifests`
- Update `README.md` with new card images if desired
- Commit: `git commit`
- Tag release: `v=$(jq .version -r package.json) git tag -s $v -m "Release $v."`
- Push changes: `git push && git push --tags`
- Publish package: `npm publish`
