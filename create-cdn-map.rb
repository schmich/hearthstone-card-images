require 'json'

cards = []

short_revs = `git rev-list master`.lines.map(&:strip).map { |rev| rev[0...4] }
if short_revs.length != short_revs.uniq.length
  raise "Short revisions collide, see 'git rev-list master'."
end

Dir['**/*.png'].each do |file|
  id = File.basename(file, '.png')
  rev = `git rev-list master -1 -- #{file}`.strip
  path = "#{rev}/#{file}"
  url = "https://cdn.rawgit.com/schmich/hearthstone-card-images/#{path}"

  short_rev = rev[0...4]
  short_path = "#{short_rev}/#{file}"
  short_url = "https://cdn.rawgit.com/schmich/hearthstone-card-images/#{short_path}"

  cards << {
    id: id,
    rev: rev,
    path: path,
    url: url,
    short_rev: short_rev,
    short_path: short_path,
    short_url: short_url
  }
end

open('cdn-map.json', 'w') do |w|
  w.write(JSON.pretty_generate(cards))
end
