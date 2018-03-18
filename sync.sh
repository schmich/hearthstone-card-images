#!/bin/bash

set -ef -o pipefail

dir="$1"
if [[ -z $dir ]]; then
  echo "Usage: $0 <images-directory>"
  exit 1
fi

if [[ ! -d $dir ]]; then
  echo "\"$dir\" does not exist or is not a directory."
  exit 1
fi

read -r -p "This will overwrite card images in \"$dir\". Continue (y/n)? "
if [[ ! $REPLY =~ ^[Yy] ]]; then
  echo Canceled.
  exit 1
fi

# Fetch latest images.json database.
echo 'Downloading latest images.json.'
json=$(curl --fail -Ss -L https://raw.githubusercontent.com/schmich/hearthstone-card-images/master/images.json)
version=$(jq -r .config.version <<< "$json")
base=$(jq -r .config.base <<< "$json")
pre=$(jq -r '.cards.pre | to_entries[] | ["pre", .key, .value] | @tsv' <<< "$json")
rel=$(jq -r '.cards.rel | to_entries[] | ["rel", .key, .value] | @tsv' <<< "$json")
cards="$pre"$'\n'"$rel"
total=$(wc -l <<< "$cards" | tr -d ' ')

index=0
while IFS=$'\t' read -r type id hash; do
  ((index++))
  echo -n "[$index/$total] "

  # Check if local image exists.
  local_file="$dir/$id.png"
  if [[ -e $local_file ]]; then
    local_hash=$(openssl sha1 -binary "$local_file" | openssl base64 | cut -c1-5)
    # Check if local image has same hash as remote image.
    if [[ $local_hash == $hash ]]; then
      echo "$local_file is up-to-date."
      continue
    fi
  fi

  # Local image doesn't exist or is different. Update it.
  url="$base/$version/$type/$id.png"
  echo "Sync $version/$type/$id.png -> $local_file."
  curl --fail -Ss -Lo "$local_file" "$url"
done <<< "$cards"
