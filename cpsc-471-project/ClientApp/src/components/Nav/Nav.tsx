import React, { useContext } from 'react';
import { Link, useHistory } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';

import './Nav.css';

const Nav = () => {
  const history = useHistory();
  const { isLoggedIn, logout } = useContext(AuthContext);

  const onLogout = () => {
    logout();
    history.replace('/login');
  };

  return (
    <div className="nav-container">
      {isLoggedIn() ? (
        <>
          <Link className="nav-container-link" to="/companies">Companies</Link>
          <Link className="nav-container-link" to="/users">Users</Link>
          <Link className="nav-container-link" to="/jobposts">Job Posts</Link>
          <a className="nav-container-link" href="#" onClick={onLogout}>Logout</a>
        </>
      ) : (
        <Link to="/login" className="nav-container-link">Login</Link>
      )}
    </div>
  );
};

export default Nav;
