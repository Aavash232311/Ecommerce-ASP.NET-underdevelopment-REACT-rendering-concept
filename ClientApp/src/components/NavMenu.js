import React, { Component } from "react";
import {
  Collapse,
  Navbar,
  NavbarBrand,
  NavbarToggler,
  NavItem,
  NavLink,
} from "reactstrap";
import { Link } from "react-router-dom";
import "./NavMenu.css";
import AuthContext, { AuthProvider } from "./auth";
import { Services } from "../utils/services";
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.utils = new Services();
    this.state = {
      collapsed: true,
    };
  }

  toggleNavbar() {
    this.setState({
      collapsed: !this.state.collapsed,
    });
  }


  render() {
    return (
      <AuthProvider>
        <AuthContext.Consumer>
          {(value) => {
            const user = value.user;
            const renderList = [
              this.utils.getUserRole() == "Seller" ? <Button key="1" onClick={() => { window.location.href = '/seller_control_panel' }} className="btn-spacing" color="inherit">Management</Button> : <Button key="1" onClick={() => { window.location.href = '/registerResturent' }} className="btn-spacing" color="inherit">Trade</Button>,
            ]
            return (
              <header>
                {/* <Navbar
                  className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3"
                  container
                  light
                >
                  <div onClick={() => { window.location.href = "/" }} id="logo">Food Portal</div>
                  <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                  <ul className="navbar-nav flex-grow">
                    {user && renderList.map((i, j) => { return (i) })}

                    <li style={{ display: user ? "none" : "block" }}>
                      <a href="/register">Register</a>
                    </li>

                    <li style={{ display: user ? "none" : "block" }}>
                      <a href="/login">Login</a>
                    </li>
                  </ul>
                </Navbar> */}
                <div>
                <Box sx={{ flexGrow: 1 }}>
                  <AppBar position="static">
                    <Toolbar>
                      <IconButton
                        size="large"
                        edge="start"
                        color="inherit"
                        aria-label="menu"
                        sx={{ mr: 2 }}
                      >
                      </IconButton>
                      <Typography onClick={() => { window.location.href = '/' }} variant="h6" component="div" sx={{ flexGrow: 1 }}>
                        DEEPBASKET
                      </Typography>
                      {user ? <Button onClick={value.logOut} color="inherit">Logout</Button > : (<div>
                        <Button onClick={() => { window.location.href = '/login' }} color="inherit">Login</Button>
                        <Button onClick={() => { window.location.href = '/register' }} color="inherit">Register</Button>
                      </div>)}
                      {user && renderList.map((i, j) => { return (i) })}
                    </Toolbar>
                  </AppBar>
                </Box>
                </div>
              </header>
            );
          }}
        </AuthContext.Consumer>
      </AuthProvider>
    );
  }
}
