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
     const [usremail, userpassword] = req.body;
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



app.get('/', (req,res)=>{
    res.send("root 경로에 서버가 성공적으로 연결되 있습니다.");
})

app.listen(PORT, ()=>{
    console.log("서버 실행중");
});