import RootButton from "./RootButton";

interface UserProfileHeaderProps {
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

export default function UserPforileComponent({
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

  return (
    <div className="user-profile-box">
      <h2 className="user-profile-username">@{username}</h2>
      <p className="user-profile-description">{description}</p>

      <div className="user-stats">
        <span>Posts: {postCount}</span>
        <span>Followers: {followerCount}</span>
        <span>Following: {followingCount}</span>
        <span>Joined: {dateFormatted}</span>
      </div>

      <RootButton
        keyLabel="F"
        onClick={onFollowToggle}
        disabled={isOwnProfile}
        backgroundColor={isFollowing ? "danger" : "primary"}
      >
        {isOwnProfile ? "T̵̟̠͗̂h̷̭̤̾͝i̷͈̎͘s̸̛͓̗ ̴̰̓̈i̴̹̽s̴̝͂̉ͅ ̴̲̊̿y̶͉̙͒̕o̸̱͑͝u̴̮̩͊̊" : isFollowing ? "Unfollow" : "Follow"}
      </RootButton>
    </div>
  );
}
