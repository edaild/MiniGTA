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

app.post('/membership', async(req, res) =>{
    const [useremail, userpassword, username] = req.body;
    try
    {
       console.log("DB에 같은 플레이어 이메일이 존재하지 않으면 회원가입 성공");
    }
    catch
    {
          res.status(500).json({success : false , message : "서버 에러 발생"});
    }
})
