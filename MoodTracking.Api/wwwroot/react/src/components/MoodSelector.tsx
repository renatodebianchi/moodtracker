import React from 'react';
import { Grid, IconButton, Typography, Box } from '@mui/material';

export type MoodOption = {
  value: number;
  label: string;
  emoji: string;
};

interface MoodSelectorProps {
  moods: MoodOption[];
  selectedMood: number | null;
  onSelect: (value: number) => void;
}

const MoodSelector: React.FC<MoodSelectorProps> = ({ moods, selectedMood, onSelect }) => (
  <Grid
    container
    spacing={{ xs: 0.5, sm: 1.5 }}
    justifyContent="space-between"
    mb={2}
    wrap="nowrap"
    sx={{
      flexDirection: { xs: 'row', sm: 'row' },
      overflowX: { xs: 'auto', sm: 'visible' },
      scrollbarWidth: 'thin',
      scrollbarColor: '#a5b4fc #e0e7ff',
      '&::-webkit-scrollbar': { height: 6 },
      '&::-webkit-scrollbar-thumb': { background: '#a5b4fc' },
    }}
  >
    {moods.map((mood) => (
      <Grid key={mood.value} sx={{ minWidth: 70, flex: 1 }}>
        <Box display="flex" flexDirection="column" alignItems="center">
          <IconButton
            onClick={() => onSelect(mood.value)}
            color={selectedMood === mood.value ? 'primary' : 'default'}
            sx={{
              border: selectedMood === mood.value ? '2px solid #6366f1' : '2px solid transparent',
              borderRadius: '50%',
              background: selectedMood === mood.value ? '#eef2ff' : 'transparent',
              transition: 'all 0.15s',
              width: { xs: 48, sm: 56 },
              height: { xs: 48, sm: 56 },
              fontSize: { xs: '1.8rem', sm: '2.2rem' },
              boxShadow: selectedMood === mood.value ? '0 2px 8px #a5b4fc55' : 'none',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
            }}
          >
            <span role="img" aria-label={mood.label} style={{ fontSize: 'inherit' }}>
              {mood.emoji}
            </span>
          </IconButton>
          <Typography
            variant="caption"
            display="block"
            textAlign="center"
            mt={0.5}
            sx={{ color: selectedMood === mood.value ? '#6366f1' : '#64748b', fontWeight: 500 }}
          >
            {mood.label}
          </Typography>
        </Box>
      </Grid>
    ))}
  </Grid>
);

export default MoodSelector;
