import { CircularProgress } from '@material-ui/core';
import axios from 'axios';
import React, { useContext, useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';
import IResume from '../../models/IResume';
import './Resume.css';

const Resume = () => {
  const { getHeaders, isInRole } = useContext(AuthContext);
  const { resumeId } = useParams<any>();
  const [resume, setResume] = useState<IResume | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (loading) return;
    setLoading(true);

    axios
      .get(`/api/resumes/${resumeId}`, getHeaders())
      .then((res) => {
        setResume(res.data);
      })
      .catch((err) => console.log(err))
      .finally(() => setLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [resumeId, getHeaders]);

  if (loading) {
    return (
      <div className="resume-container">
        <CircularProgress />
      </div>
    );
  }

  return (
    <div className="resume-container">
      {resume ? (
        <>
          <div className="resume-segment">
            <p className="resume-title">Name</p>
            <p>{resume.name}</p>
          </div>
          {isInRole('Admin') && resume.candidate && (
            <div className="resume-segment">
              <p className="resume-title">Candidate</p>
              <p>
                <Link
                  to={`/users/${resume.candidate.userName}`}
                >{`${resume.candidate.firstName} ${resume.candidate.lastName}`}</Link>
              </p>
            </div>
          )}
        </>
      ) : (
        <p>Resume does not exist :(</p>
      )}
    </div>
  );
};

export default Resume;
