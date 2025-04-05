mod lexer;

pub use crate::lexer::token_type::{punctuator, TokenType};

#[cfg(test)]
mod tests {
    use super::{punctuator, TokenType};

    #[test]
    fn get_type() {
        let mut token_type = TokenType::LeftParen;
        let mut result = punctuator(token_type);

        assert_eq!(result, Some('('));

        token_type = TokenType::Eof;
        result = punctuator(token_type);

        assert_eq!(result, None);
    }
}

