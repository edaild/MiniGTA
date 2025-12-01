require('dotenv').config();
const express = require('express');
const mysql = require('mysql2/promise');
const bodyParser = require('body-parser');
const bcrypt = require('bcrypt');

const PORT = 3000;

const app = express();
app.use(bodyParser.json());

const pool = mysql.createPool({
    host : 'localhost',
    user : 'root',
    password : '1234',
    database : 'MetroPolis'

});

app.use(express.json());

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
    console.log("고정형 데이터 베이스 서버 실행중");
});