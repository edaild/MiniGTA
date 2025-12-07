const express = require('express');
const mysql = require('mysql2/promise');
const bodyParser = require('body-parser');

const app = express();
app.use(bodyParser.json());
app.use(express.json());

const PORT = 3001;

const pool = mysql.createPool({
    host: 'localhost',
    user: 'root',
    password: '1234',
    database: 'MiniGTA'
});

app.post('/user', async (req, res) => {
    const { useremail, userpassword } = req.body;

    if (!useremail || !userpassword) {
        return res.status(400).json({ success: false, message: "이메일과 비밀번호를 모두 입력해야 합니다." });
    }

    try {
        const [rows] = await pool.query(
            'SELECT * FROM players WHERE player_email = ? AND player_password = ?',
            [useremail, userpassword]
        );

        const player = rows[0];

        if (!player) {
            return res.status(401).json({ success: false, message: "이메일 또는 비밀번호가 일치하지 않습니다." });
        }

        await pool.query(
            'UPDATE players SET player_last_login_time = CURRENT_TIMESTAMP WHERE player_id = ?',
            [player.player_id]
        );

        const userData = {
            playerId: player.player_id,
            playerName: player.player_name,
            playerLevel: player.player_level,
            currentMoney: player.current_money,
            isDead: player.is_dead,
            lastLoginTime: new Date().toISOString()
        };

        res.status(200).json({
            success: true,
            message: "로그인 성공 (평문 인증)",
            user: userData
        });
        

    } catch (error) {
        console.error("로그인 서버 에러 발생:", error);
        res.status(500).json({ success: false, message: "로그인 서버 에러 발생" });
    }
});

app.get('/', (req, res) => {
    res.send("root 경로에 서버가 성공적으로 연결되 있습니다.");
});

app.listen(PORT, () => {
    console.log(`로그인 진단 서버 실행중: http://localhost:${PORT}`);
});