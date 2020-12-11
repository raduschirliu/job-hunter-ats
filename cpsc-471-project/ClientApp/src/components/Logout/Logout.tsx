import { useContext, useEffect } from 'react';
import { useHistory } from 'react-router';
import AuthContext from '../../contexts/AuthContext';

/**
 * Logout component, deletes locally stored login credentials and redirects to login page
 */
const Logout = () => {
  const history = useHistory();
  const { logout, isLoggedIn } = useContext(AuthContext);

  useEffect(() => {
    if (isLoggedIn()) {
      logout();
    } else {
      history.replace('/login');
    }
  }, [isLoggedIn, history, logout]);

  return <p>Logging out...</p>;
};

export default Logout;
