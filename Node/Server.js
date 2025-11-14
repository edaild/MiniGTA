const express = require('express');
const mysql = require('mysql2/promise');
const PORT = 3000;

const app = express();

const pool = mysql.createPool({
    host : 'localhost',
    name : 'root',
    password : '1234',
    database : 'metropolis'

});

app.use(express.json());

app.post('/charactercard', async(req, res) =>{
    try
    {
        const[charactercard] = await pool.query(
            "SELECT character_name, character_description FROM charactercard",
        );
    }
    catch
    {
        res.status(500).json({success : false , message : error.message});
    }
});

app.post('shop', async(req, res) =>{
    try
    {
         const[shop] = await pool.query(
            "SELECT c.character_name as character_name,  character_price, charactercard_count FROM shop s JOIN characterCard c on s.charactgercard_id = c.character_id",
        );
    }
    catch
    {
         res.status(500).json({success : false , message : error.message});
    }
})

app.get('/', (req,res)=>{
    res.send("root 경로에 서버가 성공적으로 연결되 있습니다.");
})

app.listen(PORT, ()=>{
    console.log("서버 실행중");
});