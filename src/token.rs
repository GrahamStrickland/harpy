use std::slice::Iter;

#[derive(Debug)]
pub struct Token {
    pub token_type: TokenType,
    pub value: String,
}

#[derive(Clone, Copy, Debug, PartialEq)]
pub enum TokenType {
    // Punctuation and grouping
    LeftParen,
    RightParen,
    Comma,
    Assign,
    PlusEq,
    MinusEq,
    MultEq,
    DivEq,
    ModEq,
    ExpEq,
    Eq1,
    Eq2,
    Ne1,
    Ne2,
    Le,
    Ge,
    Lt,
    Gt,
    Dollar,
    Plus,
    Minus,
    Asterisk,
    Slash,
    Percent,
    Caret,
    Bang,
    Question,
    Colon,
    // Identifiers
    Name,
    // Comments
    BlockComment,
    LineComment,
    // Spacing
    Eof,
}

impl TokenType {
    pub fn compound_operator(self) -> &'static str {
        match self {
            TokenType::Assign => ":=",
            TokenType::PlusEq => "+=",
            TokenType::MinusEq => "-=",
            TokenType::MultEq => "*=",
            TokenType::DivEq => "/=",
            TokenType::ModEq => "%=",
            TokenType::ExpEq => "^=",
            TokenType::Eq1 => "==",
            TokenType::Ne2 => "!=",
            TokenType::Le => "<=",
            TokenType::Ge => ">=",
            _ => "",
        }
    }

    pub fn simple_operator(self) -> &'static str {
        match self {
            TokenType::LeftParen => "(",
            TokenType::RightParen => ")",
            TokenType::Comma => ",",
            TokenType::Eq2 => "=",
            TokenType::Ne1 => "#",
            TokenType::Lt => "<",
            TokenType::Gt => ">",
            TokenType::Dollar => "$",
            TokenType::Plus => "+",
            TokenType::Minus => "-",
            TokenType::Asterisk => "*",
            TokenType::Slash => "/",
            TokenType::Percent => "%",
            TokenType::Caret => "^",
            TokenType::Bang => "!",
            TokenType::Question => "?",
            TokenType::Colon => ":",
            _ => "",
        }
    }

    pub fn iterator() -> Iter<'static, TokenType> {
        static TOKEN_TYPES: [TokenType; 32] = [
            TokenType::LeftParen,
            TokenType::RightParen,
            TokenType::Comma,
            TokenType::Assign,
            TokenType::PlusEq,
            TokenType::MinusEq,
            TokenType::MultEq,
            TokenType::DivEq,
            TokenType::ModEq,
            TokenType::ExpEq,
            TokenType::Eq1,
            TokenType::Eq2,
            TokenType::Ne1,
            TokenType::Ne2,
            TokenType::Le,
            TokenType::Ge,
            TokenType::Lt,
            TokenType::Gt,
            TokenType::Dollar,
            TokenType::Plus,
            TokenType::Minus,
            TokenType::Asterisk,
            TokenType::Slash,
            TokenType::Percent,
            TokenType::Caret,
            TokenType::Bang,
            TokenType::Question,
            TokenType::Colon,
            TokenType::Name,
            TokenType::BlockComment,
            TokenType::LineComment,
            TokenType::Eof,
        ];

        TOKEN_TYPES.iter()
    }
}
