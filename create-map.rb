require 'json'

short_revs = `git rev-list master`.lines.map { |line| line.strip[0...4] }
if short_revs.length != short_revs.uniq.length
  raise "Short revisions collide, see 'git rev-list master'."
end

cards = Dir['**/*.png'].map do |file|
  id = File.basename(file, '.png')
  rev = `git rev-list master -1 -- #{file}`.strip[0...4]
  path = "#{rev}/#{file}"
  url = "https://cdn.rawgit.com/schmich/hearthstone-card-images/#{path}"

  { id: id, rev: rev, path: path, url: url }
end

open('map.json', 'w') do |w|
  w.write(JSON.pretty_generate(cards))
end
