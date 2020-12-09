import React, { useContext } from 'react';
import { Link } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';

import './Nav.css';

const Nav = () => {
  const { isLoggedIn } = useContext(AuthContext);

  return (
    <div className="nav-container">
      {isLoggedIn() ? (
        <>
          <Link className="nav-container-link" to="/companies">Companies</Link>
          <Link className="nav-container-link" to="/users">Users</Link>
          <Link className="nav-container-link" to="/jobposts">Job Posts</Link>
          <Link className="nav-container-link" to="/logout">Logout</Link>
        </>
      ) : (
        <>
          <Link to="/login" className="nav-container-link">Login</Link>
          <Link to="/register" className="nav-container-link">Register</Link>
        </>
      )}
    </div>
  );
};

export default Nav;
