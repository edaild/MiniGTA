const express = require('express');
const mysql = require('mysql2/promise');
const bodyParser = require('body-parser');

const app = express();
app.use(bodyParser.json());
app.use(express.json());

const PORT = 3003; 

const pool = mysql.createPool({
    host: 'localhost',
    user: 'root',
    password: '1234',
    database: 'MiniGTA'
});

app.post('/reward', async (req, res) => {
    const { useremail, npcTypeId } = req.body; 

    if (!useremail || !npcTypeId) {
        return res.status(400).json({ success: false, message: '사용자 이메일과 NPC 타입 ID가 필요합니다.' });
    }

    let connection;
    try {
        connection = await pool.getConnection();
        await connection.beginTransaction(); 

        const [playerInfo] = await connection.query(
            'SELECT player_id, current_money FROM players WHERE player_email = ?',
            [useremail]
        );

        if (playerInfo.length === 0) {
            await connection.rollback();
            return res.status(404).json({ success: false, message: '유효하지 않은 사용자 이메일입니다.' });
        }

        const playerId = playerInfo[0].player_id;
        
        const [npcInfo] = await connection.query(
            'SELECT base_money FROM npc_types WHERE npc_type_id = ?',
            [npcTypeId]
        );

        if (npcInfo.length === 0) {
            await connection.rollback();
            return res.status(404).json({ success: false, message: '알 수 없는 NPC 타입입니다.' });
        }

        const rewardAmount = npcInfo[0].base_money;
        const [updateResult] = await connection.query(
            'UPDATE players SET current_money = current_money + ? WHERE player_id = ?',
            [rewardAmount, playerId]
        );

        if (updateResult.affectedRows === 0) {
            await connection.rollback();
            return res.status(500).json({ success: false, message: 'DB 업데이트에 실패했습니다.' });
        }

        await connection.commit(); 

        const [newMoneyRow] = await connection.query('SELECT current_money FROM players WHERE player_id = ?', [playerId]);
        const newMoney = newMoneyRow[0].current_money;
        
        res.status(200).json({ 
            success: true, 
            message: `보상 ${rewardAmount}원이 지급되었습니다.`,
            rewardAmount: rewardAmount,
            newMoney: newMoney 
        });

    } catch (error) {
        if (connection) await connection.rollback(); 
        console.error("보상 지급 중 서버 에러:", error);
        res.status(500).json({ success: false, message: '보상 지급 중 서버 오류 발생' });
    } finally {
        if (connection) connection.release();
    }
});

app.get('/api/player/:useremail', async (req, res) => {
    const useremail = req.params.useremail;

    if (!useremail) {
        return res.status(400).json({ success: false, message: '사용자 이메일이 필요합니다.' });
    }

    try {
        const [rows] = await pool.query(
            'SELECT player_id, player_name, current_money, is_dead, player_last_login_time FROM players WHERE player_email = ?',
            [useremail]
        );

        if (rows.length === 0) {
            return res.status(404).json({ success: false, message: '사용자를 찾을 수 없습니다.' });
        }

        const player = rows[0];

        const userData = {
            playerId: player.player_id,
            playerName: player.player_name,
            playerLevel: player.player_level,
            currentMoney: player.current_money,
            isDead: player.is_dead,
            lastLoginTime: player.player_last_login_time
        };

        res.status(200).json({
            success: true,
            user: userData
        });

    } catch (error) {
        console.error("플레이어 정보 조회 서버 에러:", error);
        res.status(500).json({ success: false, message: '서버 오류 발생' });
    }
});

app.listen(PORT, () => {
    console.log(`게임 보상 진단 서버 실행중: http://localhost:${PORT}`);
});