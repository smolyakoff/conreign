// eslint-disable-next-line import/prefer-default-export
export function selectLayoutProps(state) {
  return {
    isPageLoading: state.operations.routePending > 0,
  };
}
