require 'json'

cards = []

Dir['**/*.png'].each do |file|
  id = File.basename(file, '.png')
  rev = `git rev-list master -1 -- #{file}`.strip
  path = "#{rev}/#{file}"
  url = "https://cdn.rawgit.com/schmich/hearthstone-card-images/#{path}"

  cards << {
    id: id,
    rev: rev,
    path: path,
    url: url
  }
end

open('cdn-map.json', 'w') do |w|
  w.write(JSON.pretty_generate(cards))
end
