const { REST, Routes, Client, GatewayIntentBits } = require('discord.js');
const fs = require("fs");
const client = new Client({ intents: [GatewayIntentBits.Guilds] });
var mysql = require('mysql');
const Long = require('long')

var mysqloptions = {
  host: "localhost",
  user: "root",
  password: "",
  database: "clashroyale"
};

var mysqlconn = mysql.createConnection(mysqloptions);
  
  

const token = require("token")
const botid = '1071462825169530980';


var tagChars = '0289PYLQGRJCUV';


const guildid = "1054159310638293046";
const rquestschannelid = "1071789536046239866";
let startTime;



const commands = [
  {
    name: 'uptime',
    description: `bot's uptime`,
  },
  {
    name: "getidbyname",
    description: `get player's id by name`,
    options: [
        {
            "name": "name",
            "description": "name to search",
            "type": 3,
            "required": true
        },
        {
          "name": "page",
          "description": "page number",
          "type": 4,
          "required": true
        }
    ]
  },
  {
    name: "getidbytag",
    description: `get player's id by name`,
    options: [
        {
            "name": "tag",
            "description": "player tag",
            "type": 3,
            "required": true
        }
    ]
  },
  {
    name: "royalereportplayer",
    description: `requests a ban for a player`,
    options: [
        {
          "name": "playerid",
          "description": "you can get with /getidbyname   or   /getidbytag",
          "type": 4,
          "required": true
        },
        {
          "name": "reason",
          "description": "report reason",
          "type": 3,
          "required": true
        }
    ]
  },
  {
    name: "addcard",
    description: `adds a card to a player`,
    options: [
        {
          "name": "playerid",
          "description": "you can get with /getidbyname   or   /getidbytag",
          "type": 4,
          "required": true
        },
        {
          "name": "cardtype",
          "description": "name to search",
          "type": 4,
          "required": true,
          "choices": [
            {
                "name": "Troop",
                "value": 26
            },
            {
                "name": "Building",
                "value": 27
            },
            {
                "name": "Spell",
                "value": 28
            }
          ]
       },
       {
         "name": "cardinstanceid",
         "description": "ID of troop (or building or spell)",
         "type": 4,
         "required": true
      }
    ]
  },
  {
    name: "banplayer",
    description: `ban a player`,
    options: [
        {
          "name": "playerid",
          "description": "you can get with /getidbyname   or   /getidbytag",
          "type": 4,
          "required": true
        },
        {
          "name": "reason",
          "description": "ban reason",
          "type": 3,
          "required": true
        }
    ]
  }
];

const rest = new REST({ version: '10' }).setToken(token);

(async () => {
  try {
    console.log('Started refreshing application (/) commands.');

    await rest.put(Routes.applicationCommands(botid), { body: commands });

    console.log('Successfully reloaded application (/) commands.');
  } catch (error) {
    console.error(error);
  }
})();

const logschannelid = "1077994913200877608";

client.once('ready', () => {
  console.log('Ready!');
  setInterval(() => {
    const data = fs.readFileSync('../src/ClashRoyale/app/lastlog.txt', 'utf8');
    if (data != "") {
        client.channels.cache.get(logschannelid).send(data);
        fs.writeFileSync('../src/ClashRoyale/app/lastlog.txt', "");
    }
  }, 5);
  startTime = Date.now();
});

client.on('interactionCreate', async interaction => {
  if (!interaction.isChatInputCommand()) return;

  if (interaction.commandName === 'uptime') {
    const uptime = (Date.now() - startTime) / 1000;
    await interaction.reply(`Uptime: ${uptime}`);
  }

  if (interaction.commandName === 'getidbyname') {
    if (interaction.options.getInteger('page', true) <= 0) {
      interaction.reply("too low page number!, try to put 1 on page number field.");
    }
    var endmessage = true;
    mysqlconn = mysql.createConnection(mysqloptions);
    mysqlconn.connect();
    mysqlconn.query(`SELECT * FROM player WHERE Home LIKE '%"name":"${interaction.options.getString('name', true)}"%' ORDER BY Trophies`, function (err, result) {
      var accountsperpage = 2;
      var messagestring = "";
      if (err) { mysqlconn.end(); return; }
      for (var i = 0; i < accountsperpage; i++)
      {
        var player = result[(interaction.options.getInteger('page', true) * accountsperpage) - accountsperpage + i]
        try {
            var home = JSON.parse(player.Home).Home;
        }catch {
          if (i == 0) {
            interaction.reply("invalid page number");
            return;
          }
        }

        try {
          messagestring += `---------------------------------\n${i}:\n**ID:** ${player.Id}\n**Name:** ${home.name}\n**Trophies:** ${player.Trophies}\n**Clan:** ${home.clan_info.name}\n **Gold:** ${home["gold"]}\n---------------------------------\n`
        }catch {
          endmessage = false;
          messagestring += "players with name **" + interaction.options.getString('name', true) + "**. (page " + interaction.options.getInteger('page', true) + "/" + ((result.length / accountsperpage) + 0.5 ) + ")\n";
          interaction.reply(messagestring + "\n!");
        }
       }
       if (endmessage) {
         messagestring += "players with name **" + interaction.options.getString('name', true) + "**. (page " + interaction.options.getInteger('page', true) + "/" + (result.length / accountsperpage) + ")\n";
         interaction.reply(messagestring + "\n!");
       }
    });
  }
  if (interaction.commandName === 'getidbytag') {
    mysqlconn = mysql.createConnection(mysqloptions);
    mysqlconn.connect();
    if (tag2id(interaction.options.getString('tag', true)) == {}) {
      interaction.reply(`invalid characters! only use 0,2,8,9,P,Y,L,Q,G,R,J,C,U,V`);
      return;
    }
    mysqlconn.query(`SELECT * FROM player WHERE Id = ${tag2id(interaction.options.getString('tag', true)).longid} ORDER BY Trophies`, function (err, result) {
      if (err) { mysqlconn.end(); return; }
      if (result.length <= 0) {
        interaction.reply(`inavid user`);
        return;
      }
      var player = result[0];
      var home = JSON.parse(player.Home).Home;
      interaction.reply(`---------------------------------\n**ID:** ${player.Id}\n**Name:** ${home.name}\n**Trophies:** ${player.Trophies}\n**Clan:** ${home.clan_info.name}\n **Gold:** ${home["gold"]}\n---------------------------------\n`);
    });
  }
  if (interaction.commandName === 'royalereportplayer') {
    mysqlconn = mysql.createConnection(mysqloptions);
    mysqlconn.connect();
    mysqlconn.query(`SELECT * FROM player WHERE Id = ${interaction.options.getInteger("playerid", true)} ORDER BY Trophies`, function (err, result) {
      try {
        var home = JSON.parse(result[0].Home).Home;
        client.channels.cache.get(rquestschannelid).send(`-----------------\n**ban request:**\n------------------\n-- ID: ${result[0].Id}\n-- Name: ${home.name}\n----------------\n**reason: ${interaction.options.getString("reason", true)}**\n**reported by: @${interaction.user.tag}**`);
        interaction.reply('Done!');
      }catch{

      }
    });
  }
  if (interaction.commandName === 'banplayer') {
    mysqlconn = mysql.createConnection(mysqloptions);
    mysqlconn.connect();
    mysqlconn.query(`SELECT * FROM player WHERE Id = ${interaction.options.getInteger("playerid", true)} ORDER BY Trophies`, function (err, result) {
      if (err) { mysqlconn.end(); return; }
      if (client.guilds.cache.get(guildid).roles.cache.get("1074129990129549312").members.get(interaction.user.id) == undefined) {
        interaction.reply("You don't have permission for that!");
        return;
      }
      try {
        var home = JSON.parse(result[0].Home).Home;
        var oldname = home.name;
        var claninfo = home.clan_info;
        mysqlconn = mysql.createConnection(mysqloptions);
        mysqlconn.connect();
        mysqlconn.query(`DELETE FROM player WHERE Id = ${interaction.options.getInteger("playerid", true)}`, function (err, result) {
          if (!err) {
            interaction.reply(`Banned Player with ID: ${interaction.options.getInteger("playerid", true)}, Name: ${oldname} and reason: ${interaction.options.getString("reason", true)}`);
          }
        });
        if (claninfo.name == "") {
          return;
        }
        var clanid = claninfo.lowId;
        mysqlconn = mysql.createConnection(mysqloptions);
        mysqlconn.connect();
        mysqlconn.query(`SELECT * FROM clan WHERE Id = ${clanid} ORDER BY Trophies`, function (err, result) {
          if (err) {throw err}
          try {
            var data = JSON.parse(result[0].Data);
            var members = data.members;
            members.forEach((member, index) => {
              if (member.lowId == interaction.options.getInteger("playerid", true)) {
                members.splice(index, 1);
              }
            });
            data.members = members;
            mysqlconn = mysql.createConnection(mysqloptions);
            mysqlconn.connect();
            mysqlconn.query(`UPDATE clan SET Data = ${data} WHERE Id = ${clanid}`, function (err, result) {
              
            });
          }catch{

          }
        });
      }catch{

      }
    });
  }

  if (interaction.commandName === 'addcard') {
    if (client.guilds.cache.get(guildid).roles.cache.get("1055446690368344154").members.get(interaction.user.id) == undefined) {
      interaction.reply("You don't have permission for that!");
      return;
    }
    mysqlconn = mysql.createConnection(mysqloptions);
    mysqlconn.connect();
    mysqlconn.query(`SELECT * FROM player WHERE Id = ${interaction.options.getInteger("playerid", true)} ORDER BY Trophies`, function (err, result) {
      if (err) { mysqlconn.end(); return; }
      try {
        var home = JSON.parse(result[0].Home).Home;
        home.deck.push({
          "Count": 1,
          "InstanceId": interaction.options.getInteger("cardinstanceid", true),
          "ClassId": interaction.options.getInteger("cardtype", true),
          "Level": 1,
          "BattleSpell": {"d":interaction.options.getInteger("cardtype", true) * 1000000 + interaction.options.getInteger("cardinstanceid", true),"l":0}
        });
        mysqlconn = mysql.createConnection(mysqloptions);
        mysqlconn.connect();
        mysqlconn.query(`UPDATE player SET Home = '${JSON.stringify({"Home": home})}' WHERE Id = ${interaction.options.getInteger("playerid", true)}`, function (err, result) {
          if (!err) {
            interaction.reply('Added Card!');
          }
        });
      }catch{
        
      }
    });
  }
});


function tag2id (tag) {
  if (tag === undefined || typeof tag !== 'string') return false

  let id = 0
  let tagArray = tag.split('')
  for (let a = 0; a < tagArray.length; a++) {
    if (tagChars.indexOf(tagArray[a]) == -1) {return {};};
    let i = tagChars.indexOf(tagArray[a])
    id *= 14
    id += i
  }
  let high = id % 256
  // let low = (id - high) >>> 8
  let low = Long.fromNumber((id - high)).shiftRight(8).low
  return {
    high: high,
    low: low,
    longid: (high << 32) | (low & 0xFFFFFFFF)
  }
}

//console.log(tag2id('9LC2L').longid);

client.login(token);

