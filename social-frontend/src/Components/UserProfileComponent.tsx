import { useNavigate } from "react-router-dom";
import RootButton from "./RootButton";

interface UserProfileHeaderProps {
  userId: number;
  username: string;
  description: string;
  createdAt: string | Date;
  postCount: number;
  followerCount: number;
  followingCount: number;
  isFollowing: boolean;
  isOwnProfile: boolean;
  onFollowToggle: () => void;
}

export default function UserProfileComponent({
  userId,
  username,
  description,
  createdAt,
  postCount,
  followerCount,
  followingCount,
  isFollowing,
  onFollowToggle,
  isOwnProfile,
}: UserProfileHeaderProps) {
  const date = new Date(createdAt);
  const dateFormatted = date.toLocaleDateString();
  const navigate = useNavigate();

  return (
    <div className="user-profile-box">
      <h2 className="user-profile-username">@{username}</h2>
      <p className="user-profile-description">{description}</p>

      <div className="d-flex mt-3 justify-content-between align-items-center">
        <div className="d-flex align-items-center gap-3 post-info">
          <div className="d-flex align-items-center gap-1">
            <i className="bi bi-file-post-fill"></i> {postCount}
          </div>
          <div className="d-flex align-items-center gap-1">
            <i className="bi bi-people-fill"></i> {followerCount}
          </div>
          <div className="d-flex align-items-center gap-1">
            <i className="bi bi-person-plus-fill"></i> {followingCount}
          </div>
          <div className="d-flex align-items-center gap-1">
            <i className="bi bi-calendar-fill"></i> {dateFormatted}
          </div>
        </div>

        <div className="d-flex gap-2">
          <RootButton
            keyLabel="F"
            fontsize={13}
            onClick={onFollowToggle}
            disabled={isOwnProfile}
            className="post-button-fixed-size"
            backgroundColor={isFollowing ? "danger" : "primary"}
          >
            {isOwnProfile ? "T̵̟̠͗̂h̷̭̤̾͝i̷͈̎͘s̸̛͓̗ ̴̰̓̈i̴̹̽s̴̝͂̉ͅ ̴̲̊̿y̶͉̙͒̕o̸̱͑͝u̴̮̩͊̊" : isFollowing ? "Unfollow" : "Follow"}
          </RootButton>
          <RootButton
            keyLabel="T"
            className="post-button-fixed-size"
            fontsize={13}
            onClick={() => navigate(`/messages/${userId}`, { state: { username } })}
            disabled={isOwnProfile}
          >
            Message
          </RootButton>
        </div>
      </div>
    </div>
  );
}
