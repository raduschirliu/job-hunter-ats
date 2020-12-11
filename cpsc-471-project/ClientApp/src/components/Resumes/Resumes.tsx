import { CircularProgress } from '@material-ui/core';
import axios from 'axios';
import React, { useContext, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';
import IResume from '../../models/IResume';

import './Resumes.css';

/**
 * ResumeCard component, responsible to display a single resume card
 * @param resume Resume object
 */
const ResumeCard = ({ resume }: { resume: IResume }) => {
  return (
    <Link to={`/resumes/${resume.resumeId}`}>
      <div className="resumes-card">
        <p>{`${resume.name}`}</p>
      </div>
    </Link>
  );
};

/*
* Resumes component, responsible to fetch and display the list of all resumes
* (either your own resumes, or those of others if admin)
*/
const Resumes = () => {
  const { getHeaders } = useContext(AuthContext);
  const [resumes, setResumes] = useState<IResume[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (loading) setLoading(true);
    axios
      .get('/api/resumes', getHeaders())
      .then((res) => {
        console.log(res.data);
        setResumes(res.data);
      })
      .catch((err) => console.log(err))
      .finally(() => setLoading(false));
  }, [loading, getHeaders]);

  return (
    <div className="resumes-container">
      {loading ? (
        <CircularProgress />
      ) : (
        <>
          {resumes.map((resume) => (
            <ResumeCard key={resume.resumeId} resume={resume} />
          ))}
        </>
      )}
    </div>
  );
};

export default Resumes;
