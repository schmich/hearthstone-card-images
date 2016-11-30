require 'json'

cards = []

Dir['**/*.png'].each do |file|
  id = File.basename(file, '.png')
  rev = `git rev-list master -1 -- #{file}`.strip

  cards << {
    id: id,
    rev: rev,
    path: "#{rev}/#{file}"
  }
end

open('cdn-map.json', 'w') do |w|
  w.write(JSON.pretty_generate(cards))
end
