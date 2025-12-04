export default interface MessageDto {
  id: number;
  sendingUserId: number;
  sendingUserName: string;
  receivingUserId: number;
  receivingUserName: string;
  content: string;
  createdAt: string;
};