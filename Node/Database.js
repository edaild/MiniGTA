const express = require('express');
const mysql = require('mysql2/promise');
const PORT = 3000;

const app = express();

const pool = mysql.createPool({
    host : 'localhost',
    user : 'root',
    password : '1234',
    database : 'MetroPolis'

});

app.use(express.json());

app.post('/user', async(req,res) =>{
     const [useremail, userpassword] = req.body;
    try
    {
       await pool.query(
        'SELECT * FROM players where player_email = ? AND player_password = ?',
        [useremail, userpassword]

       );
       
        if(players.length > 0){
            await pool.query(
                'UPDATE players SET player_last_login_time = CURRENT_TIMESTAMP WHERE player_id = ?',
                [players[0].player_id]
            );
        }
    }
    catch
    {
        res.status(500).json({success : false , message : "서버 에러 발생"});
    }
});

app.post('/membership', async(req, res) =>{
    const [useremail, userpassword, username] = req.body;
    try
    {
       
    }
    catch
    {
          res.status(500).json({success : false , message : "서버 에러 발생"});
    }
})

app.get('/charactercard', async(req, res) =>{
    try
    {
        const[charactercard] = await pool.query(
            "SELECT character_id, character_name, character_description FROM charactercard",
        );
        res.status(200).json(charactercard);

    }
    catch
    {
        res.status(500).json({success : false , message : "서버 에러 발생"});
    }
});

app.get('/shop', async(req, res) =>{
    try
    {
         const[shop] = await pool.query(
            "SELECT shop_id, c.character_name as character_name,  character_price, charactercard_count FROM shop s JOIN characterCard c on s.charactgercard_id = c.character_id",
        );
        res.status(200).json(shop);
    }
    catch
    {
         res.status(500).json({success : false , message : "서버 에러 발생"});
    }
})

app.get('/', (req,res)=>{
    res.send("root 경로에 서버가 성공적으로 연결되 있습니다.");
})

app.listen(PORT, ()=>{
    console.log("서버 실행중");
});