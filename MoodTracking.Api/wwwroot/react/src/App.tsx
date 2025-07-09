import React, { useState, useEffect, useRef } from 'react';
import { Box, Typography, Button, Paper, Stack } from '@mui/material';
import MoodSelector from './components/MoodSelector';
import type { MoodOption } from './components/MoodSelector';
import DescriptionBox from './components/DescriptionBox';
import MoodResult from './components/MoodResult';
import Login from './components/Login';
import { jwtDecode } from 'jwt-decode';
import { GoogleOAuthProvider } from '@react-oauth/google';
import MoodHistoryDialog from './components/MoodHistoryDialog';

const moods: MoodOption[] = [
	{ value: 1, label: 'Indisposto', emoji: '🥵' },
	{ value: 2, label: 'Cansado', emoji: '😪' },
	{ value: 3, label: 'Normal', emoji: '😐' },
	{ value: 4, label: 'Animado', emoji: '😁' },
	{ value: 5, label: 'Eufórico', emoji: '🤩' },
];

const BACKEND_URL = (window as any).BACKEND_URL || 'http://localhost:5164';

function App() {
	const [selectedMood, setSelectedMood] = useState<number | null>(null);
	const [descricao, setDescricao] = useState('');
	const [result, setResult] = useState<any>(null);
	const [token, setTokenState] = useState<string | null>(() => localStorage.getItem('mood_token'));
	const [historyOpen, setHistoryOpen] = useState(false);

	const resultTimeout = useRef<ReturnType<typeof setTimeout> | null>(null);

	// Limpa mensagem de registro após 10s
	useEffect(() => {
		if (result) {
			if (resultTimeout.current) clearTimeout(resultTimeout.current);
			resultTimeout.current = setTimeout(() => setResult(null), 10000);
		}
		return () => {
			if (resultTimeout.current) clearTimeout(resultTimeout.current);
		};
	}, [result]);

	// Limpa mensagem ao fazer login
	useEffect(() => {
		if (token) setResult(null);
	}, [token]);

	// Verifica expiração do token e faz logout automático
	useEffect(() => {
		if (!token) return;
		try {
			const decoded: any = jwtDecode(token);
			if (decoded && decoded.exp) {
				const now = Date.now() / 1000;
				if (decoded.exp < now) {
					setToken(null);
					return;
				}
				const timeout = setTimeout(() => setToken(null), (decoded.exp - now) * 1000);
				return () => clearTimeout(timeout);
			}
		} catch {
			setToken(null);
		}
	}, [token]);

	const setToken = (newToken: string | null) => {
		if (newToken) {
			localStorage.setItem('mood_token', newToken);
		} else {
			localStorage.removeItem('mood_token');
		}
		setTokenState(newToken);
	};

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		if (!selectedMood) {
			setResult({ error: 'Por favor, selecione um nível de humor.' });
			return;
		}
		const registro = {
			nivel: selectedMood,
			descricao: descricao || null,
			// dataHora removido: backend define a data
		};

		try {
			const resp = await fetch(`${BACKEND_URL}/api/moodentries`, {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
					'Authorization': `Bearer ${token}`,
				},
				body: JSON.stringify(registro),
			});
			if (!resp.ok) {
				const err = await resp.text();
				setResult({ error: 'Erro ao registrar humor: ' + err });
				return;
			}
			const data = await resp.json();
			setResult({ registro: data });
			setDescricao('');
			setSelectedMood(null);
		} catch (err) {
			setResult({ error: 'Erro de conexão com o backend.' });
		}
	};

	if (!token) {
		return (
			<GoogleOAuthProvider clientId={import.meta.env.VITE_GOOGLE_CLIENT_ID || '1234567890-abcdefg.apps.googleusercontent.com'}>
				<Login onLogin={setToken} backendUrl={BACKEND_URL} />
			</GoogleOAuthProvider>
		);
	}

	const handleLogout = () => {
		setToken(null);
	};

	function getUserNameFromToken(token: string | null): string {
		if (!token) return '';
		try {
			const decoded: any = jwtDecode(token);
			return decoded['name'] || decoded['unique_name'] || '';
		} catch {
			return '';
		}
	}

	const userName = getUserNameFromToken(token);

	return (
		<Box
			minHeight="100vh"
			width="100vw"
			bgcolor={"linear-gradient(135deg, #e0e7ff 0%, #f8fafc 100%)"}
			display="flex"
			alignItems="center"
			justifyContent="center"
			sx={{ p: { xs: 1, sm: 2 } }}
		>
			<Paper
				elevation={6}
				sx={{
					p: { xs: 2, sm: 4 },
					borderRadius: 4,
					minWidth: { xs: '90vw', sm: 350 },
					maxWidth: 480,
					width: '100%',
					boxShadow: '0 8px 32px 0 rgba(31, 38, 135, 0.15)',
					backdropFilter: 'blur(6px)',
					border: '1px solid #e0e7ff',
					background: 'rgba(255,255,255,0.95)',
					mx: 'auto',
				}}
			>
				<Stack spacing={2} alignItems="center">
					<Box width="100%" display="flex" justifyContent="space-between">
						<Button variant="outlined" color="primary" size="small" onClick={() => setHistoryOpen(true)} sx={{ mb: 1, borderRadius: 2 }}>
							Histórico
						</Button>
						<Button variant="outlined" color="secondary" size="small" onClick={handleLogout} sx={{ mb: 1, borderRadius: 2 }}>
							Sair
						</Button>
					</Box>
					<Typography
						variant="h6"
						fontWeight={600}
						mb={0.5}
						textAlign="center"
						sx={{ color: '#6366f1', letterSpacing: 0.5 }}
					>
						Olá{userName ? `, ${userName}` : ''}! 😊
					</Typography>
					<Typography
						variant="h5"
						fontWeight={700}
						mb={1}
						textAlign="center"
						sx={{ letterSpacing: 0.5, color: '#3730a3' }}
					>
						Como você está se sentindo hoje?
					</Typography>
					<Box component="form" width="100%" onSubmit={handleSubmit}>
						<MoodSelector moods={moods} selectedMood={selectedMood} onSelect={setSelectedMood} />
						<DescriptionBox value={descricao} onChange={setDescricao} />
						<Button
							type="submit"
							variant="contained"
							color="primary"
							fullWidth
							size="large"
							sx={{
								fontWeight: 700,
								letterSpacing: 1,
								background: 'linear-gradient(90deg, #6366f1 0%, #818cf8 100%)',
								boxShadow: '0 2px 8px #a5b4fc55',
								borderRadius: 2,
								py: 1.2,
								fontSize: 17,
								mt: 1,
								'&:hover': {
									background: 'linear-gradient(90deg, #818cf8 0%, #6366f1 100%)',
								},
							}}
						>
							Registrar humor
						</Button>
					</Box>
					<MoodResult result={result} />
					<MoodHistoryDialog open={historyOpen} onClose={() => setHistoryOpen(false)} backendUrl={BACKEND_URL} token={token} userName={userName} />
				</Stack>
			</Paper>
		</Box>
	);
}

export default App;
