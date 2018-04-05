#!/bin/bash

# Check JSON file validity.
for file in `find . -iname '*.json'`; do
  jq . "$file" >/dev/null 2>&1
  if [[ $? -ne 0 ]]; then
    echo check: $file not valid JSON.
    exit 1
  fi
done

# Check image validity.
index=0
images=`find . -iname '*.png'`
total=$(wc -l <<< "$images" | awk '{ print $1 }')
for image in $images; do
  ((index++))
  percent=$(dc -e "$index 100 * $total / p")
  printf "\r$percent%%"
  identify "$image" >/dev/null 2>&1
  if [[ $? -ne 0 ]]; then
    printf "\rcheck: $image not a valid image.\n"
    exit 1
  fi
done

printf "\rOK  \n"
