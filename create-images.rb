require 'json'
require 'progress_bar'
require 'open-uri'
require 'digest/sha1'

class Array
  include ProgressBar::WithProgress
end

def create_card_map(pattern)
  branch = `git rev-parse --abbrev-ref HEAD`.strip
  return Dir[pattern].with_progress.map { |file|
    versioned = `git ls-files #{file}`.strip
    raise "Unversioned file: #{file}." if versioned.empty?

    id = File.basename(file, '.png')
    hash = Digest::SHA1.base64digest(File.read(file))[0...5]

    [id, hash]
  }.to_h
end

package = JSON.parse(File.read('package.json'))
open('images.json', 'w') do |w|
  w.write(JSON.dump({
    config: {
      version: package['version'],
      base: 'https://raw.githubusercontent.com/schmich/hearthstone-card-images'
    },
    cards: {
      pre: create_card_map('pre/*.png').sort.to_h,
      rel: create_card_map('rel/*.png').sort.to_h
    }
  }))
end
