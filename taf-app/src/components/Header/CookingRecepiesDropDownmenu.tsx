import * as React from 'react';
import Button from '@mui/material/Button';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Fade from '@mui/material/Fade';
import {Typography } from '@mui/material';
import { Link } from 'react-router-dom';
import { KeyboardArrowDown } from '@mui/icons-material';

export default function CookingRecepieDropdownmenu() {
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);
  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };
  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <div>
      <Button
        id="fade-button"
        aria-controls={open ? 'fade-menu' : undefined}
        aria-haspopup="true"
        aria-expanded={open ? 'true' : undefined}
        onClick={handleClick}
        endIcon={<KeyboardArrowDown style={{color:"#FFFFFF"}} />}
        size='small' sx={{
            background: '#38B82E', margin: '3px'}}
      >
        <Typography color='#FFFFFF'>Recepti</Typography>
      </Button>
      <Menu
        id="fade-menu"
        MenuListProps={{
          'aria-labelledby': 'fade-button',
        }}
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        TransitionComponent={Fade}
          >
              <Link to='breakfastCookingRecepies' style={{ textDecoration: 'none' }}>
                  <MenuItem onClick={handleClose}><Typography color='#2f9a27'>Doručak</Typography></MenuItem>
              </Link>

              <Link to='lunchCookingRecepies' style={{ textDecoration: 'none' }}>
                  <MenuItem onClick={handleClose}><Typography color='#2f9a27'>Ručak</Typography></MenuItem>
              </Link>

              <Link to='dinerCookingRecepies' style={{ textDecoration: 'none' }}>
                  <MenuItem onClick={handleClose}><Typography color='#2f9a27'>Večera</Typography></MenuItem>
              </Link>

              <Link to='snacksCookingRecepies' style={{ textDecoration: 'none' }}>
                  <MenuItem onClick={handleClose}><Typography color='#2f9a27'>Užina</Typography></MenuItem>
              </Link>
          </Menu>
    </div>
  );
}