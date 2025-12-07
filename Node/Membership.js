// membership_server_diagnosis.js

const express = require('express');
const mysql = require('mysql2/promise');
const bodyParser = require('body-parser');

const app = express();
app.use(bodyParser.json());
app.use(express.json());

const PORT = 3002;
const INITIAL_MONEY = 1000;

const pool = mysql.createPool({
    host: 'localhost',
    user: 'root',
    password: '1234',
    database: 'MiniGTA'
});

app.post('/membership', async(req, res) => {
    const { useremail, userpassword, username } = req.body;

    if (!useremail || !userpassword || !username) {
        return res.status(400).json({ success: false, message: '이메일, 비밀번호, 사용자 이름을 모두 제공해야 합니다.' });
    }

    try
    { 
        const[existingPlayers] = await pool.query(
            'SELECT player_id FROM players WHERE player_email = ?',
            [useremail]
        );

        if(existingPlayers.length > 0)
        {
            return res.status(409).json({ success: false, message: "이미 존재하는 이메일 입니다." });
        } 

        const [result] = await pool.query(
            'INSERT INTO players (player_email, player_password, player_name, current_money) VALUES (?, ?, ?, ?)',
            [useremail, userpassword, username, INITIAL_MONEY]
        );

        res.status(201).json({ success: true, message: '회원 가입 성공 (평문 저장)', userId: result.insertId });
    }
    catch (error)
    {
        console.error("회원가입 서버 에러:", error);
        res.status(500).json({success : false , message : "서버 에러 발생"});
    }
});

app.listen(PORT, () => {
    console.log(`회원가입 진단 서버 실행중: http://localhost:${PORT}`);
});