import React from 'react';
import { Alert } from '@mui/material';

interface MoodResultProps {
  result: any;
}

const moodLabels: Record<number, string> = {
  1: 'Indisposto',
  2: 'Cansado',
  3: 'Normal',
  4: 'Animado',
  5: 'Eufórico',
};

const MoodResult: React.FC<MoodResultProps> = ({ result }) => {
  if (!result) return null;
  if (result.error) {
    return <Alert severity="warning" sx={{ width: '100%' }}>{result.error}</Alert>;
  }
  if (result.registro) {
    return (
      <Alert
        severity="success"
        sx={{ width: '100%', textAlign: 'left', background: '#f1f5f9', color: '#3730a3', borderRadius: 2 }}
      >
        <div>
          <b>Humor registrado com sucesso!</b>
        </div>
        <div>
          <b>Nível:</b> {moodLabels[result.registro.nivel] || result.registro.nivel}
        </div>
        <div>
          <b>Descrição:</b> {result.registro.descricao || 'Nenhuma'}
        </div>
        <div>
          <b>Data e hora:</b> {result.registro.dataHora}
        </div>
      </Alert>
    );
  }
  return null;
};

export default MoodResult;
