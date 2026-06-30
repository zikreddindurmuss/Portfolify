// AvatarUrl bilinçli olarak burada yok — avatar yönetimi ileride ayrı bir
// OBS upload akışıyla gelecek, bu feature'a dahil değil.

export interface MyProfile {
  bio: string | null;
  githubUrl: string | null;
  linkedinUrl: string | null;
  blogUrl: string | null;
}

export interface UpdateMyProfileRequest {
  bio: string | null;
  githubUrl: string | null;
  linkedinUrl: string | null;
  blogUrl: string | null;
}

/// Public profil sayfası (portfolify.app/{slug}) için — tamamen herkese açık.
export interface PublicProfile {
  username: string;
  slug: string;
  bio: string | null;
  githubUrl: string | null;
  linkedinUrl: string | null;
  blogUrl: string | null;
}
