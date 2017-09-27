require 'json'
require 'progress_bar'
require 'open-uri'

class Array
  include ProgressBar::WithProgress
end

short_revs = `git rev-list master`.lines.map { |line| line.strip[0...4] }
if short_revs.length != short_revs.uniq.length
  raise "Short revisions collide, see 'git rev-list master'."
end

database = JSON.parse(open('https://api.hearthstonejson.com/v1/latest/enUS/cards.json').read)
ids = database.map { |card| [card['dbfId'], card['id']] }.to_h

branch = `git rev-parse --abbrev-ref HEAD`.strip
cards = Dir['**/*.png'].with_progress.map do |file|
  dbf_id = File.basename(file, '.png').to_i
  raise 'Invalid DBF ID.' if dbf_id == 0

  rev = `git rev-list #{branch} -1 -- #{file}`.strip[0...4]
  path = "#{rev}/#{file}"
  url = "https://github.com/schmich/hearthstone-card-images/raw/#{path}"

  { id: ids[dbf_id], dbfId: dbf_id, url: url }
end

open('images.json', 'w') do |w|
  w.write(JSON.pretty_generate(cards))
end
