SHELL := /bin/bash

update-images:
	ruby create-images.rb
	mvimdiff <(git show master:images.json | jq -r .) <(cat images.json | jq -r .)

.PHONY: update-images
