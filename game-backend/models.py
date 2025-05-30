from pydantic import BaseModel
from sqlalchemy import Column, Integer, String
from db import Base

# SQLAlchemy DB model
class User(Base):
    __tablename__ = "users"
    id = Column(Integer, primary_key=True, index=True)
    username = Column(String, unique=True, index=True)
    hashed_password = Column(String)
    balance = Column(Integer, default=1000)

# Pydantic schemas
class UserIn(BaseModel):
    username: str
    password: str

class UserOut(BaseModel):
    username: str
    balance: int