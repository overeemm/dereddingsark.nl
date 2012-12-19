require 'net/ftp'
require 'net/http'
require 'net/https'
require 'uri'
require File.expand_path(File.dirname(__FILE__) + '/config.rb')
require File.expand_path(File.dirname(__FILE__) + '/categories.rb')

categories = Array.new

categories[0] = Category.new("samenkomst baarn", "public_html/preken/samenkomsten", "preken/samenkomsten")
categories[1] = Category.new("samenkomst bunschoten", "public_html/preken/samenkomsten", "preken/samenkomsten")
categories[2] = Category.new("jeugddienst", "public_html/preken/jeugddiensten", "preken/jeugddiensten")
categories[3] = Category.new("maranatha avond", "public_html/preken/maranatha", "preken/maranatha")
categories[4] = Category.new("bijbelstudies", "public_html/bijbelstudies", "bijbelstudies")

filePath = ARGV[0]
fileName = File.basename(filePath)
tempFilePath = filePath + "small.mp3"

categories.length.times do |i|
  puts "  " + i.to_s + " " + categories[i].name
end
puts "> Welke categorie?\n"

categoryIndex = STDIN.gets.strip.to_i
category = categories[categoryIndex]

dummy = category.name.to_s

puts "> Welke spreker?\n"
spreker = STDIN.gets.strip

puts "> Welke titel?\n"
titel = STDIN.gets.strip

puts "> Welke datum (yyyy-MM-dd HH:mm:ss)?\n"
datum = STDIN.gets.strip

targetFileName = spreker
unless titel.nil? or titel.empty?
  targetFileName = spreker + " - " + titel
end
friendlyName = targetFileName
targetFileName = datum[0, 4] + datum[5, 2] + datum[8, 2] + "-" + targetFileName
targetFileName = targetFileName.gsub(/[\. \x00\/\\:\*\?\"<>\|]/, '_').downcase + ".mp3"

puts "> Start encoding " + fileName
File.delete(tempFilePath) if File.exists?(tempFilePath)
result = `lame -V9 -b 32 -h "#{filePath}" "#{tempFilePath}"`

puts "> FTP naar " + FTPURL
ftp = Net::FTP.new
ftp.connect(FTPURL, 21)
ftp.login(FTPUSERNAME, FTPPASSWORD)

puts "> FTP upload " + tempFilePath + " to " + category.ftppath
ftp.chdir(category.ftppath)
ftp.put(tempFilePath, targetFileName)
ftp.close

httplocation = FTPSITEADDRESS + category.sitepath + "/" + targetFileName

puts "> Find the file at " + httplocation 

postbody = "url="+ httplocation + "&name=" + friendlyName + "&datetime=" + datum + "&categorie=" + category.name
uri = URI.parse(APIADDRESS)
https = Net::HTTP.new(uri.host, uri.port)
https.set_debug_output $stderr
https.use_ssl = true

req, body = https.post(uri.path, postbody, {"X-UserGuid" => APIUSERGUID, "X-Token" => APIUSERTOKEN})
req.each{ |h,v| puts "#{h}: #{v}" }

puts "> Send /audio/add request, #{body.size} bytes received"

File.delete(tempFilePath) if File.exists?(tempFilePath)