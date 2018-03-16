require 'json'
require 'progress_bar'
require 'open-uri'
require 'digest/sha1'

class Array
  include ProgressBar::WithProgress
end

short_revs = `git rev-list master`.lines.map { |line| line.strip[0...4] }
if short_revs.length != short_revs.uniq.length
  raise "Short revisions collide, see 'git rev-list master'."
end

def create_card_map(pattern)
  branch = `git rev-parse --abbrev-ref HEAD`.strip
  return Dir[pattern].with_progress.map { |file|
    rev = `git rev-list #{branch} -1 -- #{file}`.strip[0...4]
    raise "Unversioned file: #{file}." if rev.empty?

    dbf_id = File.basename(file, '.png')
    dir = File.dirname(file)
    path = "#{rev}/#{dir}"
    hash = Digest::SHA1.base64digest(File.read(file))[0...5]

    [dbf_id, [path, hash]]
  }.to_h
end

prerelease = create_card_map('pre/*.png')
release = create_card_map('rel/*.png')

open('images.json', 'w') do |w|
  w.write(JSON.dump({
    config: {
      version: `jq -r .version package.json`.strip,
      base: 'https://raw.githubusercontent.com/schmich/hearthstone-card-images/'
    },
    cards: prerelease.merge(release)
  }))
end
