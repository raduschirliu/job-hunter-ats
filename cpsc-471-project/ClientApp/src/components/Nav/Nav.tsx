import React, { useContext, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';

import './Nav.css';

/**
 * Nav bar component, responsible for displaying different nav links based on whether the 
 * user is currently authenticated or not
 */
const Nav = () => {
  const { getUser, isLoggedIn } = useContext(AuthContext);
  const [name, setName] = useState('');

  useEffect(() => {
    setName(getUser()?.firstName + ' ' + getUser()?.lastName);
  }, [getUser]);

  return (
    <div className="nav-container">
      {isLoggedIn() ? (
        <>
          <Link className="nav-container-link" to="/companies">
            Companies
          </Link>
          <Link className="nav-container-link" to="/users">
            Users
          </Link>
          <Link className="nav-container-link" to="/jobposts">
            Job Posts
          </Link>
          <Link className="nav-container-link" to="/resumes">
            Resumes
          </Link>

          <div className="nav-container-right">
            <Link className="nav-container-link" to={`/users/${getUser()?.userName}`}>
              {`Hi ${name}`}
            </Link>
            <Link className="nav-container-link" to="/logout">
              Logout
            </Link>
          </div>
        </>
      ) : (
        <>
          <Link to="/login" className="nav-container-link">
            Login
          </Link>
          <Link to="/register" className="nav-container-link">
            Register
          </Link>
        </>
      )}
    </div>
  );
};

export default Nav;
