import React from 'react';
import { TextField } from '@mui/material';

interface DescriptionBoxProps {
  value: string;
  onChange: (value: string) => void;
}

const DescriptionBox: React.FC<DescriptionBoxProps> = ({ value, onChange }) => (
  <TextField
    label="Descreva o que está sentindo (opcional)"
    multiline
    minRows={2}
    maxRows={4}
    fullWidth
    value={value}
    onChange={(e) => onChange(e.target.value)}
    variant="outlined"
    sx={{ mb: 2, background: '#f1f5f9', borderRadius: 2 }}
    InputProps={{ style: { fontSize: 15 } }}
  />
);

export default DescriptionBox;
